using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Common;
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
		[NotNull] private const string DefaultConsoleApplicationExtension = "exe";
		[NotNull] private const string Message = "Execute T4 design-time template";

		[NotNull]
		private string TemporaryExecutableNameBase => FileName.WithoutExtension();

		protected override string DestinationFileName { get; }

		[NotNull]
		// TODO: store it in output folder
		// TODO: use file name
		private FileSystemPath TemporaryExecutableFilePath =>
			DestinationFilePath.Parent.Combine("T4CompilationAssemblyName.exe");

		private FileSystemPath GetTemporaryExecutablePath([NotNull] IT4Environment environment) => Project
			.GetOutputDirectory(environment.TargetFrameworkId)
			.SelectFreshName(TemporaryExecutableNameBase, DefaultConsoleApplicationExtension);

		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private string TargetExtension { get; }

		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
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
		[CanBeNull]
		private LifetimeDefinition LaunchProgress([NotNull] IProgressIndicator progress, [NotNull] ISolution solution)
		{
			if (!(progress is IProgressIndicatorModel model)) return null;
			progress.Start(1);
			progress.Advance();
			var task = RiderBackgroundTaskBuilder
				.FromProgressIndicator(model)
				.AsIndeterminate()
				.WithHeader("Executing template")
				.Build();
			var taskHost = solution.GetComponent<RiderBackgroundTaskHost>();

			var solutionLifetime = solution.GetLifetime();
			var lifetimeDefinition = solutionLifetime.CreateNested();
			taskHost.AddNewTask(lifetimeDefinition.Lifetime, task);
			return lifetimeDefinition;
		}

		private void PerformBackgroundWork(
			[NotNull] ISolution solution,
			[NotNull] IProjectFile destinationFile,
			[CanBeNull] LifetimeDefinition definition
		)
		{
			string code = new T4CSharpExecutableCodeGenerator(File, Manager).Generate().Builder.ToString();
			Compile(solution, code);
			Execute();
			Cleanup(code);
			definition?.Terminate();
		}

		// TODO: do NOT store all the data in a single string. It might be huge.
		// TODO: use progress indicator
		private void Compile([NotNull] ISolution solution, [NotNull] string code)
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));


			// I need mscorlib and System.CodeDom
			// TODO: avoid memory leaks
			var reference1 = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			var reference2 = MetadataReference.CreateFromFile(typeof(CompilerError).Assembly.Location);

			var compilation =
				CSharpCompilation.Create("T4CompilationAssemblyName.exe",
					new[] {syntaxTree},
					options: options,
					references: new[] {reference1, reference2});

			var errors = (compilation.GetDiagnostics()
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.Select(diagnostic => diagnostic.ToString())).ToList();
			if (!errors.IsEmpty())
			{
				EmitErrors(errors);
				return;
			}

			FileSystemDefinition.CreateTemporaryFile(Lifetime.Eternal);

			// TODO: also generate pdb
			compilation.Emit(TemporaryExecutableFilePath.FullPath);
		}

		private void Execute()
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
