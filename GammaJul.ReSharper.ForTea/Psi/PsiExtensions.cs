using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	internal static class PsiExtensions {

		[CanBeNull]
		internal static IPsiSourceFile FindSourceFileInSolution([CanBeNull] this FileSystemPath includePath, [CanBeNull] ISolution solution) {
			if (includePath == null || includePath.IsEmpty || solution == null)
				return null;

			IProjectFile includeProjectfile = solution
				.FindProjectItemsByLocation(includePath)
				.OfType<IProjectFile>()
				.FirstOrDefault();

			return includeProjectfile != null ? includeProjectfile.ToSourceFile() : null;
		}

	}

}