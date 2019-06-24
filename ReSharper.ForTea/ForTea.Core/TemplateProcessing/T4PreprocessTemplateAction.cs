using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.Extension;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;
using static GammaJul.ForTea.Core.TemplateProcessing.T4CSharpCodeGenerationUtils;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	[ContextAction(
		Name = "PreprocessTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public class T4PreprocessTemplateAction : ContextActionBase
	{
		[NotNull] public const string DefaultDestinationFileName = "TextTransformation.cs";
		[NotNull] private const string Message = "Preprocess T4 template";

		[CanBeNull]
		private IT4File File { get; }

		[CanBeNull]
		private IPsiSourceFile PsiSourceFile => File?.GetSourceFile();

		[CanBeNull]
		private IProjectFile ProjectFile => PsiSourceFile?.ToProjectFile();

		[CanBeNull]
		private FileSystemPath FilePath => PsiSourceFile?.GetLocation();

		[CanBeNull]
		private IProjectFolder ProjectFolder => ProjectFile?.ParentFolder;

		[CanBeNull]
		private FileSystemPath ProjectFolderPath => FilePath?.Parent;

		[CanBeNull]
		private IProjectFile DestinationProjectFile =>
			ProjectFolder?.GetSubItems(DestinationFileName).SingleItem() as IProjectFile;

		[CanBeNull]
		private string DestinationFileName =>
			File?.GetSourceFile()?.Name.WithExtension(PreprocessResultExtension) ?? DefaultDestinationFileName;

		private FileSystemPath DestinationFilePath => ProjectFolderPath?.Combine(DestinationFileName);

		public T4PreprocessTemplateAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) =>
			File = dataProvider.PsiFile as IT4File;

		[NotNull]
		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => _ => solution.InvokeUnderTransaction(cookie =>
		{
			if (File == null) return;
			var manager = solution.GetComponent<T4DirectiveInfoManager>();
			var generator = new T4CSharpCodeGenerator(File, manager);
			string message = generator.Generate().Builder.ToString();
			var destinationFile = GetOrCreateDestinationFile(cookie);
			if (destinationFile == null) return;
			// TODO: use better writing methods
			using (var writeStream = destinationFile.CreateWriteStream())
			{
				writeStream.WriteUtf8(message);
			}

			cookie.Commit(progress);
		});

		[CanBeNull]
		private IProjectFile GetOrCreateDestinationFile([NotNull] IProjectModelTransactionCookie cookie)
		{
			var result = DestinationProjectFile;
			if (result != null) return result;
			Assertion.Assert(ProjectFolder != null, "ProjectFolder != null");
			if (!cookie.CanAddFile(ProjectFolder, DestinationFilePath, out string _)) return null;
			return cookie.AddFile(ProjectFolder, DestinationFilePath);
		}

		public override string Text => Message;
		public override bool IsAvailable(IUserDataHolder cache) => File != null;
	}
}
