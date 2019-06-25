using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.Extension;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.Interactive.Csi;
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
	public sealed class T4GenerateTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string Message = "Execute T4 design-time template";
		protected override string DestinationFileName { get; }
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private string TargetExtension { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4GenerateTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
			Manager = dataProvider.Solution.GetComponent<T4DirectiveInfoManager>();
			TargetExtension = File?.GetTargetExtension(Manager) ?? T4CSharpCodeGenerationUtils.DefaultTargetExtension;
			DestinationFileName = FileName?.WithOtherExtension(TargetExtension);
			Logger = JetBrains.Util.Logging.Logger.GetLogger<T4GenerateTemplateContextAction>();
		}

		[CanBeNull]
		private FileSystemPath FindFreshScriptPath([CanBeNull] string fileName) =>
			// We expect to receive answer "<FileName>.tmp.csx"
			ProjectFolderPath?.Combine(FindFreshScriptName(fileName + ".tmp"));

		[CanBeNull]
		// File name is NOT supposed to have extension
		private string FindFreshScriptName([CanBeNull] string fileName)
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
		private void GenerateCode([NotNull] string newFilePath)
		{
			if (File == null) throw new InvalidOperationException();
			var generator = new T4CSharpInteractiveCodeGenerator(File, Manager);
			string message = generator.Generate().Builder.ToString();

			using (var stream = System.IO.File.Create(newFilePath))
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(message);
			}
		}

		private void SaveResult([NotNull] ISolution solution, [NotNull] string result) =>
			solution.InvokeUnderTransaction(cookie =>
			{
				var destinationFile = GetOrCreateDestinationFile(cookie);
				if (destinationFile == null) return;
				// TODO: use better writing methods
				using (var writeStream = destinationFile.CreateWriteStream())
				{
					writeStream.WriteUtf8(result);
				}
			});

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			string tmpFilePath = FindFreshScriptPath(FileName?.WithoutExtension())?.FullPath;
			if (tmpFilePath == null) throw new InvalidOperationException();

			try
			{
				GenerateCode(tmpFilePath);
				string result = Execute(tmpFilePath, solution);
				SaveResult(solution, result);
			}
			catch (Exception e)
			{
				Logger.Log(
					LoggingLevel.ERROR,
					"Could not write data to temporary file",
					e.WithSensitiveData("tmpFilePath", tmpFilePath));
			}
			finally
			{
				// This will not delete user files, since the name is fresh
				Cleanup(tmpFilePath);
			}
		});

		// TODO: do this in background
		// TODO: remove logging
		private string Execute([NotNull] string tmpFilePath, [NotNull] ISolution solution)
		{
			Logger.Log(LoggingLevel.VERBOSE, $"Starting executing {tmpFilePath}");
			var detector = solution.GetComponent<CSharpInteractiveDetector>();
			Logger.Log(LoggingLevel.VERBOSE, $"Got csi detector: {detector}");
			var toolPath = detector.DetectInteractiveToolPath(solution.GetSettingsStore());
			string toolFullPath = toolPath.FullPath;
			Logger.Log(LoggingLevel.VERBOSE, $"Detected csi path: {toolFullPath}");
			var defaultArguments = CSharpInteractiveDetector.GetDefaultArgumentsForTool(toolPath);
			string arguments = JoinArguments(defaultArguments, tmpFilePath);
			Logger.Log(LoggingLevel.VERBOSE, $"Arguments: {arguments}");

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					FileName = toolFullPath,
					Arguments = arguments,
					CreateNoWindow = true
				}
			};
			Logger.Log(LoggingLevel.VERBOSE, "Starting csi process...");
			process.Start();
			Logger.Log(LoggingLevel.VERBOSE, "Started process");
			string output = process.StandardOutput.ReadToEnd();
			Logger.Log(LoggingLevel.VERBOSE, $"stdout: {output}");
			if (output.IsNullOrEmpty())
			{
				Logger.Log(LoggingLevel.VERBOSE, $"stdout is bad");
				output = process.StandardError.ReadToEnd();
				Logger.Log(LoggingLevel.VERBOSE, $"stderr: {output}");
			}

			Logger.Log(LoggingLevel.VERBOSE, "Waiting for process to exit...");
			process.WaitForExit();
			Logger.Log(LoggingLevel.VERBOSE, "Process exited");
			return output;
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

		private void Cleanup([NotNull] string newFilePath)
		{
			try
			{
				System.IO.File.Delete(newFilePath);
			}
			catch (Exception)
			{
				// Ignore
			}
		}

		public override string Text => Message;
	}
}
