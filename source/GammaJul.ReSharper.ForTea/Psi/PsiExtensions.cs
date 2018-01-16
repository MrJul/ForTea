#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
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

		internal static bool IsPreprocessedT4Template([CanBeNull] this IProjectFile projectFile) {
			string customTool = projectFile.GetCustomTool();
			return "TextTemplatingFilePreprocessor".Equals(customTool, StringComparison.OrdinalIgnoreCase);
		}

		[CanBeNull]
		private static ProjectFileProperties TryGetProperties([CanBeNull] IProjectFile projectFile) {
			return projectFile != null ? projectFile.Properties as ProjectFileProperties : null;
		}

		[CanBeNull]
		internal static string GetCustomTool([CanBeNull] this IProjectFile projectFile) {
			ProjectFileProperties properties = TryGetProperties(projectFile);
			return properties != null ? properties.CustomTool : null;
		}

		[CanBeNull]
		internal static string GetCustomToolNamespace([CanBeNull] this IProjectFile projectFile) {
			ProjectFileProperties properties = TryGetProperties(projectFile);
			return properties != null ? properties.CustomToolNamespace : null;
		}

		internal static void MarkAsDirty([NotNull] this IPsiServices psiServices, [NotNull] IPsiSourceFile psiSourcefile) {
			psiServices.Files.MarkAsDirty(psiSourcefile);
			psiServices.Caches.MarkAsDirty(psiSourcefile);
		}

		[CanBeNull]
		internal static IReferenceName GetUsedNamespaceNode([CanBeNull] this IUsingDirective directive) {
			var usingSymbolDirective = directive as IUsingSymbolDirective;
			if (usingSymbolDirective == null || usingSymbolDirective.StaticKeyword != null)
				return null;
			
			return usingSymbolDirective.ImportedSymbolName;
		}

	}

}