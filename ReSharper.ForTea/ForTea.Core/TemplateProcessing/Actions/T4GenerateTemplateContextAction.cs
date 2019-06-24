using System;
using System.Diagnostics;
using System.IO;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.Interactive.Csi;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.TextControl;
using JetBrains.Util;
using JetBrains.Util.Logging;

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

		public T4GenerateTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
			Manager = dataProvider.Solution.GetComponent<T4DirectiveInfoManager>();
			TargetExtension = File?.GetTargetExtension(Manager) ?? T4CSharpCodeGenerationUtils.DefaultTargetExtension;
			DestinationFileName = FileName?.WithOtherExtension(TargetExtension);
		}

		[CanBeNull]
		private FileSystemPath FindFreshPath([CanBeNull] string fileName) =>
			ProjectFolderPath?.Combine(FindFreshName(fileName + ".tmp"));

		[CanBeNull]
		// File name is NOT supposed to have extension
		private string FindFreshName([CanBeNull] string fileName)
		{
			if (ProjectFolderPath == null) return null;
			if (fileName == null) return null;

			// First, try simple name and hope it works
			var path = ProjectFolderPath.Combine(fileName.WithExtension(T4CSharpCodeGenerationUtils.CSharpExtension));
			if (!path.ExistsFile) return fileName;

			// Well, damn it
			var logger = Logger.GetLogger<T4GenerateTemplateContextAction>();
			for (int index = 2;; index += 1)
			{
				string newFileName = (fileName + index).WithExtension(T4CSharpCodeGenerationUtils.CSharpExtension);
				logger.Log(LoggingLevel.WARN,
					$"Experiencing issues finding a good name for temporary file. Trying name {newFileName}...");
				path = ProjectFolderPath.Combine(newFileName.WithExtension(TargetExtension));
				if (!path.ExistsFile) return newFileName;
			}
		}

		/// <summary>
		/// Creates tmp file and writes data into it.
		/// The file is not supposed to be visible and thus not added to project model.
		/// </summary>
		private static void WriteData([NotNull] string newFilePath, [NotNull] string message)
		{
			using (var stream = System.IO.File.Create(newFilePath))
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(message);
			}
		}

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			if (File == null) return;

			string message = new T4CSharpCodeGenerator(File, Manager).Generate().Builder.ToString();
			var tmpFilePath = FindFreshPath(FileName?.WithoutExtension());

			if (tmpFilePath == null) return;

			string tmpFileFullPath = tmpFilePath.FullPath;
			WriteData(tmpFileFullPath, message);
			Execute(tmpFileFullPath, solution);
			Cleanup(tmpFileFullPath);
		});

		private void Execute([NotNull] string tmpFilePath, [NotNull] ISolution solution)
		{
			var detector = solution.GetComponent<CSharpInteractiveDetector>();
			var toolPath = detector.DetectInteractiveToolPath(solution.GetSettingsStore());
			var defaultArguments = CSharpInteractiveDetector.GetDefaultArgumentsForTool(toolPath);

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = "cmd.exe",
					Arguments = "/C echo 'Hello, world!'",
					RedirectStandardOutput = true
				}
			};
			process.Start();
			
			throw new NotImplementedException();
		}

		private void Cleanup([NotNull] string newFilePath)
		{
			// Delete this file before anyone notices something happening!
			System.IO.File.Delete(newFilePath);
		}

		public override string Text => Message;
	}
}
