using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.Actions
{
	public abstract class T4FileBasedContextActionBase : ContextActionBase
	{
		[CanBeNull]
		protected IT4File File { get; }

		[CanBeNull]
		protected IPsiSourceFile PsiSourceFile => File?.GetSourceFile();

		[CanBeNull]
		protected IProjectFile ProjectFile => PsiSourceFile?.ToProjectFile();

		[CanBeNull]
		protected FileSystemPath FilePath => PsiSourceFile?.GetLocation();

		[CanBeNull]
		protected string FileName => PsiSourceFile?.Name;
		
		[CanBeNull]
		protected IProjectFolder ProjectFolder => ProjectFile?.ParentFolder;

		[CanBeNull]
		protected FileSystemPath ProjectFolderPath => FilePath?.Parent;

		[CanBeNull]
		protected IProjectFile DestinationProjectFile =>
			ProjectFolder?.GetSubItems(DestinationFileName).SingleItem() as IProjectFile;

		[CanBeNull]
		protected FileSystemPath DestinationFilePath => ProjectFolderPath?.Combine(DestinationFileName);

		[CanBeNull]
		protected IProjectFile GetOrCreateDestinationFile([NotNull] IProjectModelTransactionCookie cookie)
		{
			var result = DestinationProjectFile;
			if (result != null) return result;
			Assertion.Assert(ProjectFolder != null, "ProjectFolder != null");
			Assertion.Assert(DestinationFilePath != null, "DestinationFilePath != null");
			if (!cookie.CanAddFile(ProjectFolder, DestinationFilePath, out string _)) return null;
			return cookie.AddFile(ProjectFolder, DestinationFilePath);
		}

		protected T4FileBasedContextActionBase([CanBeNull] IT4File file) => File = file;

		[CanBeNull]
		protected abstract string DestinationFileName { get; }

		public sealed override bool IsAvailable(IUserDataHolder cache) =>
			File?.ContainsErrorElement() == false;
	}
}
