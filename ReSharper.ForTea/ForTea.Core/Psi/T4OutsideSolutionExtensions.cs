using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {

	public static class T4OutsideSolutionExtensions {

		[NotNull] private static readonly Key<FileSystemPath> _outsideSolutionPathKey = new Key<FileSystemPath>("OutsideSolutionPath");

		public static void SetOutsideSolutionPath([NotNull] this IDocument document, [NotNull] FileSystemPath fileSystemPath)
			=> document.PutData(_outsideSolutionPathKey, fileSystemPath);

		[NotNull]
		public static FileSystemPath GetOutsideSolutionPath([NotNull] this IDocument document)
			=> document.GetData(_outsideSolutionPathKey) ?? FileSystemPath.Empty;

	}

}