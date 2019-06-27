using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.BackgroundTasks;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.TextControl;
using JetBrains.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4ExecuteTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string DefaultExecutableExtension = "exe";
		[NotNull] private const string Message = "Execute T4 design-time template";
		[NotNull] private const string DefaultExecutableExtensionWithDot = "." + DefaultExecutableExtension;
		protected override string DestinationFileName { get; }

		[NotNull]
		private FileSystemPath CreateTemporaryExecutable(Lifetime lifetime) =>
			FileSystemDefinition.CreateTemporaryFile(lifetime, extensionWithDot: DefaultExecutableExtensionWithDot);

		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private string TargetExtension { get; }

		/// See
		/// <see cref="JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.T4FileBasedContextActionBase">
		/// base class
		/// </see>
		/// constructor for details
		[SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized"),
		 SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse"),
		 SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
			if (File == null) return;
			Manager = dataProvider.Solution.GetComponent<T4DirectiveInfoManager>();
			TargetExtension = File.GetTargetExtension(Manager);
			DestinationFileName = FileName.WithOtherExtension(TargetExtension);
		}

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			Check();
			var lifetimeDefinition = LaunchProgress(progress, solution);
			var destinationFile = GetOrCreateDestinationFile(cookie);
			PerformBackgroundWork(solution, destinationFile, lifetimeDefinition);
		});

		// TODO: add more informative messages to progress indicator
		[NotNull]
		private LifetimeDefinition LaunchProgress(
			[NotNull] IProgressIndicator progress,
			[NotNull] ISolution solution
		)
		{
			var solutionLifetime = solution.GetLifetime();
			var definition = solutionLifetime.CreateNested();
			if (!(progress is IProgressIndicatorModel model)) return definition;
			progress.Start(1);
			progress.Advance();
			var task = RiderBackgroundTaskBuilder
				.FromProgressIndicator(model)
				.AsIndeterminate()
				.WithHeader("Executing template")
				.Build();
			solution.GetComponent<RiderBackgroundTaskHost>().AddNewTask(definition.Lifetime, task);
			return definition;
		}

		private void PerformBackgroundWork(
			[NotNull] ISolution solution,
			[NotNull] IProjectFile destinationFile,
			[NotNull] LifetimeDefinition definition
		)
		{
			string code = new T4CSharpExecutableCodeGenerator(File, Manager).Generate().RawText;
			var executablePath = CreateExecutable(solution, code, definition.Lifetime);
			if (executablePath == null)
			{
				definition.Terminate();
				return;
			}

			Execute(executablePath);
			Cleanup(code);
			definition.Terminate();
		}

		// TODO: do NOT store all the data in a single string. It might be huge.
		// TODO: use progress indicator
		[CanBeNull]
		private FileSystemPath CreateExecutable(
			[NotNull] ISolution solution,
			[NotNull] string code,
			Lifetime lifetime
		)
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));


			var compilation =
				CSharpCompilation.Create("T4CompilationAssemblyName.exe",
					new[] {syntaxTree},
					options: options,
					references: ExtractReferences());

			var errors = compilation.GetDiagnostics()
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.Select(diagnostic => diagnostic.ToString()).ToList();
			if (!errors.IsEmpty())
			{
				EmitErrors(errors);
				return null;
			}


			// TODO: also generate pdb
			var executablePath = CreateTemporaryExecutable(lifetime);
			compilation.Emit(executablePath.FullPath);
			return executablePath;
		}

		private IEnumerable<MetadataReference> ExtractReferences() =>
			Solution
				.GetComponent<IPsiModules>()
				.GetModuleReferences(PsiSourceFile.PsiModule)
				.Select(it => it.Module)
				.OfType<IAssemblyPsiModule>()
				.Select(it => it.Assembly)
				.SelectNotNull(it => it.Location)
				.Select(it => MetadataReference.CreateFromFile(it.FullPath));

		private void Execute([NotNull] FileSystemPath executablePath)
		{
#if false
			var detector = solution.GetComponent<CSharpInteractiveDetector>();
			var patcher = solution.GetComponent<RiderProcessStartInfoPatcher>();
			var toolPath = detector.DetectInteractiveToolPath(solution.GetSettingsStore());
			string toolFullPath = toolPath.FullPath;
			var defaultArguments = CSharpInteractiveDetector.GetDefaultArgumentsForTool(toolPath);
			string arguments = JoinArguments(defaultArguments, tmpFilePath);
			var startInfo = new JetProcessStartInfo(new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = toolFullPath,
				Arguments = arguments,
				CreateNoWindow = true
			});
			// TODO: should this one be used?
			var request = JetProcessRuntimeRequest.CreateFramework();
			var patchResult = patcher.Patch(startInfo, request);
			var process = new Process {StartInfo = patchResult.GetPatchedInfoOrThrow().ToProcessStartInfo()};
			process.Start();

			string output = await process.StandardOutput.ReadToEndAsync();
			if (output.IsNullOrEmpty())
			{
				output = await process.StandardError.ReadToEndAsync();
			}

			await process.WaitForExitAsync();
			return output;
#endif
		}

		private void EmitErrors([NotNull, ItemNotNull] IEnumerable<string> errors)
		{
		}

		private void Cleanup([NotNull] string tmpFilePath)
		{
			try
			{
				System.IO.File.Delete(tmpFilePath);
			}
			catch (Exception)
			{
				// Ignore
			}
		}

		public override string Text => Message;
	}
}
