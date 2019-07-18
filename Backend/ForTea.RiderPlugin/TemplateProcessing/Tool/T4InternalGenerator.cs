using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Altering.Resources;
using JetBrains.ReSharper.Host.Features.ProjectModel.CustomTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Tool
{
	[ShellComponent] // TODO: make SolutionComponent
	public class T4InternalGenerator : ISingleFileCustomTool
	{
		public string Name => "Bundled T4 template executor";
		public string ActionName => "Execute T4 template";
		public IconId Icon => FileLayoutThemedIcons.TypeTemplate.Id;
		public string[] CustomTools => new[] {"T4 Generator Custom Tool"};
		public string[] Extensions => new[] {T4ProjectFileType.MainExtensionNoDot};
		public bool IsEnabled => true;

		public bool IsApplicable(IProjectFile projectFile)
		{
			using (projectFile.Locks.UsingReadLock())
			{
				return projectFile.LanguageType.Is<T4ProjectFileType>();
			}
		}

		public string[] Keywords => new[] {"tt", "t4", "template"};

		public ISingleFileCustomToolExecutionResult Execute(IProjectFile projectFile)
		{
			AssertOperationValidity(projectFile);

			var file = AsT4File(projectFile).NotNull("file != null");
			var solution = projectFile.GetSolution();
			var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
			var targetFileManager = solution.GetComponent<IT4TargetFileManager>();
			
			string result = executionManager.Execute(file); // Write to buffer
			var affectedFile = targetFileManager.SaveResults(result, file); // Write disk entry

			return new SingleFileCustomToolExecutionResult(new[] {affectedFile}, EmptyList<string>.Collection);
		}

		private static void AssertOperationValidity([NotNull] IProjectFile projectFile)
		{
			projectFile.AssertIsValid();
			projectFile.Locks.AssertReadAccessAllowed();
			projectFile.GetSolution().GetPsiServices().Files.AssertAllDocumentAreCommitted();
		}

		[CanBeNull]
		private static IT4File AsT4File([CanBeNull] IProjectFile projectFile) =>
			projectFile?.ToSourceFile()?.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleOrDefault();
	}
}
