using System.Linq;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.FileTypes;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Selection {

	/// <summary>Support for extend selection (Ctrl+W).</summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4SelectEmbracingConstructProvider : ISelectEmbracingConstructProvider {

		public bool IsAvailable(IPsiSourceFile sourceFile)
			=> sourceFile.Properties.ShouldBuildPsi;

		public ISelectedRange GetSelectedRange(IPsiSourceFile sourceFile, DocumentRange documentRange) {
			(IT4File t4File, IFile codeBehindFile) = GetFiles(sourceFile, documentRange);

			ITreeNode t4Node = t4File?.FindNodeAt(documentRange);
			if (t4Node == null)
				return null;

			// if the current selection is inside C# code, use the C# extend selection directly
			if (codeBehindFile != null) {
				ISelectEmbracingConstructProvider codeBehindProvider = PsiShared.GetComponent<PsiProjectFileTypeCoordinator>()
					.GetByPrimaryPsiLanguageType(codeBehindFile.Language)
					.SelectNotNull(fileType => Shell.Instance.GetComponent<IProjectFileTypeServices>().TryGetService<ISelectEmbracingConstructProvider>(fileType))
					.FirstOrDefault();

				ISelectedRange codeBehindRange = codeBehindProvider?.GetSelectedRange(sourceFile, documentRange);
				if (codeBehindRange != null)
					return new T4CodeBehindWrappedSelection(t4File, codeBehindRange);
			}

			return new T4NodeSelection(t4File, t4Node);
		}

		private static Pair<IT4File, IFile> GetFiles([NotNull] IPsiSourceFile sourceFile, DocumentRange documentRange) {
			IT4File primaryFile = null;
			IFile secondaryFile = null;

			foreach ((IFile file, _) in sourceFile.EnumerateIntersectingPsiFiles(documentRange)) {
				if (file is IT4File t4File)
					primaryFile = t4File;
				else
					secondaryFile = file;
			}

			return Pair.Of(primaryFile, secondaryFile);
		}

	}

}