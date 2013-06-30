using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4MiscFilesProjectPsiModuleProvider {
		
		public void OnProjectFileChanged(IProjectFile projectFile, PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder) {
			_t4PsiModuleProvider.OnProjectFileChanged(projectFile, ref changeType, changeBuilder);
		}

	}

}