using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Provides <see cref="T4PsiModule"/> for T4 files opened outside of the solution.
	/// </summary>
	[MiscFilesProjectPsiModuleProvider]
	public sealed class T4MiscFilesProjectPsiModuleProvider : IMiscFilesProjectPsiModuleProvider {
		private readonly T4PsiModuleProvider _t4PsiModuleProvider;
		
		public IEnumerable<IPsiModule> GetModules() {
			return _t4PsiModuleProvider.GetModules();
		}

		public IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile) {
			return _t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile);
		}

		public void OnProjectFileChanged(IProjectFile projectFile, PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder) {
			if (projectFile.LanguageType.Is<T4ProjectFileType>())
				_t4PsiModuleProvider.OnProjectFileChanged(projectFile, changeType, changeBuilder);
		}

		public void Dispose() {
			_t4PsiModuleProvider.Dispose();
		}

		public T4MiscFilesProjectPsiModuleProvider([NotNull] T4PsiModuleProvider t4PsiModuleProvider) {
			_t4PsiModuleProvider = t4PsiModuleProvider;
		}

	}

}