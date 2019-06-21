using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	[ContextAction(
		Name = "ProcessTemplate",
		Description = "Generate T4 runtime template",
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public class GenerateTemplateContextAction : ContextActionBase
	{
		private IT4File File { get; }

		public GenerateTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) =>
			File = dataProvider.PsiFile as IT4File;

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			var manager = solution.GetComponent<T4DirectiveInfoManager>();
			var provider = solution.GetComponent<T4TemplateBaseProvider>();
			
			IPsiSourceFile psiSourceFile = File.GetSourceFile();
			string fileName = psiSourceFile?.Name;
			IProjectFile projectFile = psiSourceFile.ToProjectFile();
			IProjectFolder parentFolder = projectFile?.ParentFolder;
			FileSystemPath projectFolderPath = parentFolder?.Path?.ReferencedFolderPath;
			FileSystemPath newFile = projectFolderPath?.Combine("");

			if (newFile == null) return;
//			if (!cookie.CanAddFile(parentFolder, file, out string reason))
//			{
//				throw new ProjectModelEditorException(reason);
//			}

//			var generator = new T4CSharpCodeGenerator(File, Manager, Provider, false);
		});

		public override string Text => "Generate T4 runtime template";
		public override bool IsAvailable(IUserDataHolder cache) => File != null;
	}
}
