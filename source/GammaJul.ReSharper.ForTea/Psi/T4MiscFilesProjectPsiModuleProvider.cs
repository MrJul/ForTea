using JetBrains.Util;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>Provides <see cref="T4PsiModule"/> for T4 files opened outside of the solution.</summary>
	[MiscFilesProjectPsiModuleProvider]
	public sealed class T4MiscFilesProjectPsiModuleProvider : IMiscFilesProjectPsiModuleProvider {

		[NotNull] private readonly T4PsiModuleProvider _t4PsiModuleProvider;

		public IEnumerable<IPsiModule> GetModules()
			=> _t4PsiModuleProvider.GetModules();

		public IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile)
			=> _t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile);

		public void Dispose()
			=> _t4PsiModuleProvider.Dispose();

		public void OnProjectFileChanged(
			IProjectFile projectFile,
			PsiModuleChange.ChangeType changeType,
			PsiModuleChangeBuilder changeBuilder,
			FileSystemPath oldLocation
		)
			=> _t4PsiModuleProvider.OnProjectFileChanged(projectFile, ref changeType, changeBuilder);

		public T4MiscFilesProjectPsiModuleProvider(
			[NotNull] Lifetime lifetime,
			[NotNull] IShellLocks shellLocks,
			[NotNull] ChangeManager changeManager,
			[NotNull] T4Environment t4Environment
		) {
			_t4PsiModuleProvider = new T4PsiModuleProvider(lifetime, shellLocks, changeManager, t4Environment);
		}

	}

}