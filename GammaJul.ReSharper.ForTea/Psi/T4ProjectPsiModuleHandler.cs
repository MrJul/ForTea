using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Provides <see cref="T4PsiModule"/> for T4 files opened inside the solution.
	/// </summary>
	internal sealed class T4ProjectPsiModuleHandler : DelegatingProjectPsiModuleHandler {
		private readonly T4PsiModuleProvider _t4PsiModuleProvider;

		public override IList<IPsiModule> GetAllModules() {
			var modules = new List<IPsiModule>(base.GetAllModules());
			modules.AddRange(_t4PsiModuleProvider.GetModules());
			return modules;
		}

		public override void OnProjectFileChanged(IProjectFile projectFile, FileSystemPath oldLocation, PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder) {
			if (projectFile.LanguageType.Is<T4ProjectFileType>())
				_t4PsiModuleProvider.OnProjectFileChanged(projectFile, changeType, changeBuilder);
			else
				base.OnProjectFileChanged(projectFile, oldLocation, changeType, changeBuilder);
		}

		public override IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile) {
			return base.GetPsiSourceFilesFor(projectFile).Concat(_t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile));
		}

		public T4ProjectPsiModuleHandler([NotNull] IProjectPsiModuleHandler handler, T4PsiModuleProvider t4PsiModuleProvider)
			: base(handler) {
			_t4PsiModuleProvider = t4PsiModuleProvider;
		}

	}

}