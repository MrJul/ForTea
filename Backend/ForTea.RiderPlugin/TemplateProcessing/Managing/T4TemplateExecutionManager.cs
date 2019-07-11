using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using JetBrains.Util.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	[SolutionComponent]
	public sealed class T4TemplateExecutionManager : IT4TemplateExecutionManager
	{
		[NotNull] private const string DefaultExecutableExtension = "exe";
		[NotNull] private const string DefaultExecutableExtensionWithDot = "." + DefaultExecutableExtension;
		[NotNull] private const string Title = "Executing T4 Template";
		[NotNull] private const string ErrorMessage = "ErrorGeneratingOutput";

		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		[NotNull]
		private IT4TargetFileManager TargetFileManager { get; }

		[NotNull]
		private ISolution Solution { get; }

		private Lifetime SolutionLifetime { get; }

		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private IPsiModules PsiModules { get; }

		[NotNull]
		private ISolutionProcessStartInfoPatcher Patcher { get; }

		[NotNull]
		private IPsiFiles PsiFiles { get; }

		public T4TemplateExecutionManager(
			Lifetime solutionLifetime,
			[NotNull] IShellLocks locks,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] IPsiModules psiModules,
			[NotNull] ISolutionProcessStartInfoPatcher patcher,
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] IPsiFiles psiFiles
		)
		{
			SolutionLifetime = solutionLifetime;
			Locks = locks;
			DirectiveInfoManager = directiveInfoManager;
			PsiModules = psiModules;
			Patcher = patcher;
			Solution = solution;
			TargetFileManager = targetFileManager;
			PsiFiles = psiFiles;
		}

		public void Execute(IT4File file, IProgressIndicator progress = null, Lifetime? outerLifetime = null)
		{
			var baseLifetime = outerLifetime ?? SolutionLifetime;
			var definition = baseLifetime.CreateNested();
			LaunchProgress(progress);
			var info = GenerateCode(file, progress);
			CompileAndRun(definition, info);
		}

		private static void LaunchProgress([CanBeNull] IProgressIndicator indicator)
		{
			if (indicator == null) return;
			indicator.Start(1);
			indicator.Advance();
			indicator.TaskName = Title;
			indicator.CurrentItemText = "Preparing";
		}

		private T4TemplateExecutionManagerInfo GenerateCode(
			[NotNull] IT4File file,
			[CanBeNull] IProgressIndicator progress
		)
		{
			if (progress != null) progress.CurrentItemText = "Generating code";
			using (ReadLockCookie.Create())
			{
				var timeStamp = file.GetSourceFile().NotNull().LastWriteTimeUtc;
				var generator = new T4CSharpExecutableCodeGenerator(file, DirectiveInfoManager);
				string code = generator.Generate().RawText;
				var references = ExtractReferences(file);
				return new T4TemplateExecutionManagerInfo(timeStamp, code, references, file, progress);
			}
		}

		private IEnumerable<MetadataReference> ExtractReferences([NotNull] IT4File file)
		{
			Locks.AssertReadAccessAllowed();
			var sourceFile = file.GetSourceFile().NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var psiModule = sourceFile.PsiModule;
			var resolveContext = psiModule.GetResolveContextEx(projectFile);
			using (CompilationContextCookie.GetOrCreate(resolveContext))
			{
				return PsiModules
					.GetModuleReferences(psiModule)
					.Select(it => it.Module)
					.OfType<IAssemblyPsiModule>()
					.Select(it => it.Assembly)
					.SelectNotNull(it => it.Location)
					.Select(it => MetadataReference.CreateFromFile(it.FullPath));
			}
		}

		private void CompileAndRun(LifetimeDefinition definition, T4TemplateExecutionManagerInfo info)
		{
			if (info.ProgressIndicator != null) info.ProgressIndicator.CurrentItemText = "Compiling code";
			var executablePath = CreateTemporaryExecutable(definition.Lifetime);
			var compilation = CreateCompilation(info);
			var errors = compilation
				.GetDiagnostics(definition.Lifetime)
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.ToList();
			if (!errors.IsEmpty())
			{
				ReportError(definition, info, errors);
				return;
			}

			compilation.Emit(executablePath.FullPath, cancellationToken: definition.Lifetime);
			CopyAssemblies(info, executablePath);
			Run(info, definition, executablePath);
		}

		private CSharpCompilation CreateCompilation(T4TemplateExecutionManagerInfo info)
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				info.Code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

			// TODO: use actual name
			return CSharpCompilation.Create(
				"T4CompilationAssemblyName.exe",
				new[] {syntaxTree},
				options: options,
				references: info.References);
		}

		private void ReportError(
			LifetimeDefinition definition,
			T4TemplateExecutionManagerInfo info,
			IList<Diagnostic> errors
		)
		{
			MessageBox.ShowError(errors.Select(error => error.ToString()).Join("\n"), "Could not compile template");
			SaveExecutionResult(definition, info, ErrorMessage);
		}

		private void CopyAssemblies(
			T4TemplateExecutionManagerInfo info,
			[NotNull] FileSystemPath executablePath
		)
		{
			var folder = executablePath.Parent;
			IEnumerable<FileSystemPath> query = info
				.References
				.SelectNotNull(it => it.Display)
				.SelectNotNull(it => FileSystemPath.TryParse(it));
			foreach (var path in query)
			{
				File.Copy(path.FullPath, folder.Combine(path.Name).FullPath);
			}
		}

		private void Run(
			T4TemplateExecutionManagerInfo info,
			LifetimeDefinition definition,
			FileSystemPath executablePath
		)
		{
			var lifetime = definition.Lifetime;
			var process = LaunchProcess(lifetime, executablePath);
			lifetime.ThrowIfNotAlive();
			process.WaitForExitSpinning(100, info.ProgressIndicator);
			lifetime.ThrowIfNotAlive();
			string stdout = process.StandardOutput.ReadToEnd();
			lifetime.ThrowIfNotAlive();
			string stderr = process.StandardError.ReadToEnd();
			lifetime.ThrowIfNotAlive();
			SaveExecutionResult(definition, info, stderr.IsNullOrEmpty() ? stdout : ErrorMessage);
		}

		private Process LaunchProcess(Lifetime lifetime, FileSystemPath executablePath)
		{
			var startInfo = new JetProcessStartInfo(new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = executablePath.FullPath,
				CreateNoWindow = true
			});

			var request = JetProcessRuntimeRequest.CreateFramework();
			var patchedInfo = Patcher.Patch(startInfo, request).GetPatchedInfoOrThrow();
			var process = new Process
			{
				StartInfo = patchedInfo.ToProcessStartInfo()
			};
			lifetime.OnTermination(process);
			lifetime.Bracket(
				() => process.Start(),
				() => Logger.CatchSilent(() =>
				{
					if (!process.HasExited) process.KillTree();
				})
			);
			return process;
		}

		private void SaveExecutionResult(LifetimeDefinition definition, T4TemplateExecutionManagerInfo info,
			[NotNull] string result)
		{
			Locks.ExecuteOrQueueReadLockEx(definition.Lifetime, "Saving T4 Template Execution Results", () =>
			{
				try
				{
					info.AssertFileHasNotChanged();
					using (WriteLockCookie.Create())
					{
						Solution.InvokeUnderTransaction(cookie =>
						{
							TargetFileManager.GetOrCreateDestinationFile(info.File, cookie);
							var destination = TargetFileManager.GetDestinationPath(info.File);
							// TODO: fix endings!
							destination.WriteAllText(result.Replace("\r\n", "\n"));
							cookie.Commit(NullProgressIndicator.Create());
						});
					}
				}
				finally
				{
					definition.Terminate();
				}
			});
		}

		[NotNull]
		private static FileSystemPath CreateTemporaryExecutable(Lifetime lifetime)
		{
			var directory = FileSystemDefinition.CreateTemporaryDirectory();
			return FileSystemDefinition.CreateTemporaryFile(lifetime, directory, DefaultExecutableExtensionWithDot);
		}
	}
}
