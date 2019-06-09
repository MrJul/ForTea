using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi
{
	/// <summary>Provides <see cref="IT4PsiModule"/> for T4 files opened inside the solution.</summary>
	internal sealed class T4ProjectPsiModuleHandler : DelegatingProjectPsiModuleHandler
	{
		[NotNull] private readonly T4PsiModuleProvider _t4PsiModuleProvider;

		public override IList<IPsiModule> GetAllModules()
		{
			var modules = new List<IPsiModule>(base.GetAllModules());
			modules.AddRange(_t4PsiModuleProvider.GetModules());

			return modules;
		}

		public override void OnProjectFileChanged(
			IProjectFile projectFile,
			FileSystemPath oldLocation,
			PsiModuleChange.ChangeType changeType,
			PsiModuleChangeBuilder changeBuilder
		)
		{
			if (!_t4PsiModuleProvider.OnProjectFileChanged(projectFile, ref changeType, changeBuilder))
				base.OnProjectFileChanged(projectFile, oldLocation, changeType, changeBuilder);
		}

		public override IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile)
			=> base.GetPsiSourceFilesFor(projectFile).Concat(_t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile));

		public T4ProjectPsiModuleHandler(
			Lifetime lifetime,
			[NotNull] IProjectPsiModuleHandler handler,
			[NotNull] ChangeManager changeManager,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IProject project,
			[NotNull] IT4PsiModuleFactory moduleFactory
		) : base(handler) => _t4PsiModuleProvider = new T4PsiModuleProvider(
			lifetime,
			project.Locks,
			changeManager,
			t4Environment,
			moduleFactory
		);
	}
}
