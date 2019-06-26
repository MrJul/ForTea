using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.BackgroundTasks;
using JetBrains.ReSharper.Host.Features.Interactive.Csi;
using JetBrains.ReSharper.Host.Features.Processes;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4ExecuteTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string Message = "Execute T4 design-time template";
		protected override string DestinationFileName { get; }
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private string TargetExtension { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
			Manager = dataProvider.Solution.GetComponent<T4DirectiveInfoManager>();
			TargetExtension = File?.GetTargetExtension(Manager) ?? T4CSharpCodeGenerationUtils.DefaultTargetExtension;
			DestinationFileName = FileName?.WithOtherExtension(TargetExtension);
			Logger = JetBrains.Util.Logging.Logger.GetLogger<T4ExecuteTemplateContextAction>();
		}

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			var lifetimeDefinition = LaunchProgress(progress, solution);
			string tmpFilePath = FindFreshTmpFilePath(FileName?.WithoutExtension())?.FullPath;
			if (tmpFilePath == null) throw new InvalidOperationException();
			GenerateCode(tmpFilePath);
			var destinationFile = GetOrCreateDestinationFile(cookie);
			Assertion.Assert(destinationFile != null, "destinationFile != null");
			PerformBackgroundWorkAsync(solution, tmpFilePath, destinationFile, lifetimeDefinition);
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

		private async void PerformBackgroundWorkAsync(
			[NotNull] ISolution solution,
			[NotNull] string tmpFilePath,
			[NotNull] IProjectFile destinationFile,
			[CanBeNull] LifetimeDefinition definition
		)
		{
			string result = await ExecuteScriptAsync(solution, tmpFilePath);
			await SaveResultAsync(destinationFile, result);
			await Cleanup(tmpFilePath);
			definition?.Terminate();
		}

		// TODO: remove logging
		// TODO: do NOT store all the data in a single string. It might be huge.
		// TODO: use progress indicator
		private async Task<string> ExecuteScriptAsync([NotNull] ISolution solution, [NotNull] string tmpFilePath)
		{
			var detector = solution.GetComponent<CSharpInteractiveDetector>();
			Logger.Log(LoggingLevel.WARN, $"Got csi detector: {detector}");
			var patcher = solution.GetComponent<RiderProcessStartInfoPatcher>();
			Logger.Log(LoggingLevel.WARN, $"Got process start info patcher: {patcher}");
			var toolPath = detector.DetectInteractiveToolPath(solution.GetSettingsStore());
			string toolFullPath = toolPath.FullPath;
			Logger.Log(LoggingLevel.WARN, $"Detected csi path: {toolFullPath}");
			var defaultArguments = CSharpInteractiveDetector.GetDefaultArgumentsForTool(toolPath);
			string arguments = JoinArguments(defaultArguments, tmpFilePath);
			Logger.Log(LoggingLevel.WARN, $"Arguments: {arguments}");
			var startInfo = new JetProcessStartInfo(new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = toolFullPath,
				Arguments = arguments,
				CreateNoWindow = true
			});
			Logger.Log(LoggingLevel.WARN, $"Created jet start info: {startInfo}");
			// TODO: should this one be used?
			var request = JetProcessRuntimeRequest.CreateFramework();
			Logger.Log(LoggingLevel.WARN, $"Created request: {request}");
			var patchResult = patcher.Patch(startInfo, request);
			Logger.Log(LoggingLevel.WARN, $"Patched start info. Result: {patchResult}");
			var process = new Process {StartInfo = patchResult.GetPatchedInfoOrThrow().ToProcessStartInfo()};
			Logger.Log(LoggingLevel.WARN, $"Created process: {process}");
			process.Start();

			Logger.Log(LoggingLevel.WARN, "Started process");
			string output = await process.StandardOutput.ReadToEndAsync();
			Logger.Log(LoggingLevel.WARN, $"stdout: {output}");
			if (output.IsNullOrEmpty())
			{
				Logger.Log(LoggingLevel.WARN, $"stdout is bad");
				output = await process.StandardError.ReadToEndAsync();
				Logger.Log(LoggingLevel.WARN, $"stderr: {output}");
			}

			Logger.Log(LoggingLevel.WARN, "Waiting for process to exit...");
			await process.WaitForExitAsync();
			Logger.Log(LoggingLevel.WARN, "Process exited");
			return output;
		}

		[CanBeNull]
		private FileSystemPath FindFreshTmpFilePath([CanBeNull] string fileName) =>
			// We expect to receive answer "<FileName>.tmp.csx"
			ProjectFolderPath?.Combine(FindFreshTmpFileName(fileName + ".tmp"));

		[CanBeNull]
		// File name is NOT supposed to have extension
		private string FindFreshTmpFileName([CanBeNull] string fileName)
		{
			if (ProjectFolderPath == null) return null;
			if (fileName == null) return null;

			// First, try simple name and hope it works
			string candidate = fileName.WithExtension(T4CSharpCodeGenerationUtils.CSharpInteractiveExtension);
			var path = ProjectFolderPath.Combine(candidate);
			if (!path.ExistsFile) return candidate;

			// Well, damn it
			for (int index = 2;; index += 1)
			{
				candidate = (fileName + index).WithExtension(T4CSharpCodeGenerationUtils.CSharpInteractiveExtension);
				Logger.Log(LoggingLevel.WARN,
					$"Experiencing issues finding a good name for temporary file. Trying name {candidate}...");
				path = ProjectFolderPath.Combine(candidate.WithExtension(TargetExtension));
				if (!path.ExistsFile) return candidate;
			}
		}

		/// <summary>
		/// Creates tmp file and writes data into it.
		/// The file is not supposed to be visible and thus not added to project model.
		/// </summary>
		private void GenerateCode([NotNull] string tmpFilePath)
		{
			Assertion.Assert(File != null, "File != null");
			var generator = new T4CSharpInteractiveCodeGenerator(File, Manager);
			string message = generator.Generate().Builder.ToString();

			using (var stream = System.IO.File.Create(tmpFilePath))
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(message);
			}
		}

		private async Task SaveResultAsync([NotNull] IProjectFile projectFile, [NotNull] string result)
		{
			using (var stream = projectFile.CreateWriteStream())
			using (var writer = new StreamWriter(stream))
			{
				await writer.WriteAsync(result);
			}
		}

		[NotNull]
		private string JoinArguments(
			[NotNull, ItemNotNull] IEnumerable<string> defaultArguments,
			[NotNull] string tmpFilePath
		)
		{
			var result = new StringBuilder();
			foreach (string argument in defaultArguments)
			{
				result.Append(argument);
				result.Append(' ');
			}

			result.Append(tmpFilePath);
			return result.ToString();
		}

		private async Task Cleanup([NotNull] string tmpFilePath)
		{
			try
			{
				// TODO: find asynchronous API
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
