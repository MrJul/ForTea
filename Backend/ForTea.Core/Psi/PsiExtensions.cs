using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {
	
	internal static class PsiExtensions {

		[CanBeNull]
		public static IPsiSourceFile FindSourceFileInSolution([CanBeNull] this FileSystemPath includePath, [CanBeNull] ISolution solution) {
			if (includePath == null || includePath.IsEmpty)
				return null;

			return solution
				?.FindProjectItemsByLocation(includePath)
				.OfType<IProjectFile>()
				.FirstOrDefault()?.ToSourceFile();
		}

		public static bool IsPreprocessedT4Template([CanBeNull] this IProjectFile projectFile) {
			string customTool = projectFile.GetCustomTool();
			return "TextTemplatingFilePreprocessor".Equals(customTool, StringComparison.OrdinalIgnoreCase);
		}

		[CanBeNull]
		private static ProjectFileProperties TryGetProperties([CanBeNull] IProjectFile projectFile)
			=> projectFile?.Properties as ProjectFileProperties;

		[CanBeNull]
		public static string GetCustomTool([CanBeNull] this IProjectFile projectFile)
			=> TryGetProperties(projectFile)?.CustomTool;

		[CanBeNull]
		public static string GetCustomToolNamespace([CanBeNull] this IProjectFile projectFile)
			=> TryGetProperties(projectFile)?.CustomToolNamespace;

		public static void MarkAsDirty([NotNull] this IPsiServices psiServices, [NotNull] IPsiSourceFile psiSourcefile) {
			psiServices.Files.MarkAsDirty(psiSourcefile);
			psiServices.Caches.MarkAsDirty(psiSourcefile);
		}

		[CanBeNull]
		public static IReferenceName GetUsedNamespaceNode([CanBeNull] this IUsingDirective directive)
			=> directive is IUsingSymbolDirective usingSymbolDirective && usingSymbolDirective.StaticKeyword == null
				? usingSymbolDirective.ImportedSymbolName
				: null;

	}

}