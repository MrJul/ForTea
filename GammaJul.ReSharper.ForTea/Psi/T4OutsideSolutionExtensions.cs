using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	public static class T4OutsideSolutionExtensions {

		private static readonly Key<FileSystemPath> _outsideSolutionPathKey = new Key<FileSystemPath>("OutsideSolutionPath");

		public static void SetOutsideSolutionPath([NotNull] this IDocument document, [NotNull] FileSystemPath fileSystemPath) {
			document.PutData(_outsideSolutionPathKey, fileSystemPath);
		}

		[NotNull]
		public static FileSystemPath GetOutsideSolutionPath([NotNull] this IDocument document) {
			return document.GetData(_outsideSolutionPathKey) ?? FileSystemPath.Empty;
		}

	}

}