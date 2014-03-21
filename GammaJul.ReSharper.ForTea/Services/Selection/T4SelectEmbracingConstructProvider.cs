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


using System.Linq;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.Lazy;
#if SDK80
using JetBrains.ProjectModel.FileTypes;
#endif

namespace GammaJul.ReSharper.ForTea.Services.Selection {

	/// <summary>
	/// Support for extend selection (Ctrl+W).
	/// </summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4SelectEmbracingConstructProvider : ISelectEmbracingConstructProvider {

		public bool IsAvailable(IPsiSourceFile sourceFile) {
			return sourceFile.Properties.ShouldBuildPsi;
		}

		public ISelectedRange GetSelectedRange(IPsiSourceFile sourceFile, DocumentRange documentRange) {
			Pair<IT4File, IFile> pair = GetFiles(sourceFile, documentRange);
			IT4File t4File = pair.First;
			IFile codeBehindFile = pair.Second;

			if (t4File == null)
				return null;

			ITreeNode t4Node = t4File.FindNodeAt(documentRange);
			if (t4Node == null)
				return null;

			// if the current selection is inside C# code, use the C# extend selection directly
			if (codeBehindFile != null) {
				ISelectEmbracingConstructProvider codeBehindProvider = PsiShared.GetComponent<PsiProjectFileTypeCoordinator>()
					.GetByPrimaryPsiLanguageType(codeBehindFile.Language)
					.SelectNotNull(fileType => Shell.Instance.GetComponent<IProjectFileTypeServices>().TryGetService<ISelectEmbracingConstructProvider>(fileType))
					.FirstOrDefault();

				if (codeBehindProvider != null) {
					ISelectedRange codeBehindRange = codeBehindProvider.GetSelectedRange(sourceFile, documentRange);
					if (codeBehindRange != null)
						return new T4CodeBehindWrappedSelection(t4File, codeBehindRange);
				}
			}

			return new T4NodeSelection(t4File, t4Node);
		}

		private static Pair<IT4File, IFile> GetFiles([NotNull] IPsiSourceFile sourceFile, DocumentRange documentRange) {
			IT4File primaryFile = null;
			IFile secondaryFile = null;

			foreach (Pair<IFile, TreeTextRange> pair in sourceFile.EnumerateIntersectingPsiFiles(documentRange)) {
				var t4File = pair.First as IT4File;
				if (t4File != null)
					primaryFile = t4File;
				else
					secondaryFile = pair.First;
			}

			return Pair.Of(primaryFile, secondaryFile);
		}

	}

}