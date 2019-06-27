using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions
{
	public abstract class T4FileBasedContextActionBase : ContextActionBase
	{
		private bool IsValid { get; }

		[NotNull]
		protected IT4File File { get; }

		[NotNull]
		protected IPsiSourceFile PsiSourceFile { get; }

		[NotNull]
		protected IProjectFile ProjectFile { get; }

		[NotNull]
		protected IProject Project { get; }

		[NotNull]
		protected FileSystemPath FilePath => PsiSourceFile.GetLocation();

		[NotNull]
		protected string FileName => PsiSourceFile.Name;

		[CanBeNull]
		protected IProjectFolder ProjectFolder => ProjectFile.ParentFolder;

		[NotNull]
		protected FileSystemPath ProjectFolderPath => FilePath.Parent;

		[CanBeNull]
		protected IProjectFile DestinationProjectFile =>
			ProjectFolder?.GetSubItems(DestinationFileName).SingleItem() as IProjectFile;

		[NotNull]
		protected FileSystemPath DestinationFilePath => ProjectFolderPath.Combine(DestinationFileName);

		[NotNull]
		protected IProjectFile GetOrCreateDestinationFile([NotNull] IProjectModelTransactionCookie cookie)
		{
			var result = DestinationProjectFile;
			if (result != null) return result;
			Assertion.Assert(ProjectFolder != null, "ProjectFolder != null");
			Assertion.Assert(DestinationFilePath != null, "DestinationFilePath != null");
			if (!cookie.CanAddFile(ProjectFolder, DestinationFilePath, out string reason))
				throw new InvalidOperationException(reason);
			return cookie.AddFile(ProjectFolder, DestinationFilePath);
		}

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification =
			"If any propery is null, IsValid (and IsAvailable) will be false, and no methods will be called")]
		protected T4FileBasedContextActionBase([CanBeNull] IT4File file)
		{
			IsValid = true;
			var psiSourceFile = file?.GetSourceFile();
			var projectFile = psiSourceFile?.ToProjectFile();
			var project = projectFile?.GetProject();
			if (project == null) IsValid = false;
			File = file;
			PsiSourceFile = psiSourceFile;
			ProjectFile = projectFile;
			Project = project;
			if (File.ContainsErrorElement()) IsValid = false;
		}

		[NotNull]
		protected abstract string DestinationFileName { get; }

		public sealed override bool IsAvailable(IUserDataHolder cache) => IsValid;

		[Conditional("JET_MODE_ASSERT")]
		protected void Check() => Assertion.Assert(IsValid, "IsValid");
	}
}
