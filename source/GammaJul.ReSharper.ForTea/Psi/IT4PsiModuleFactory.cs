using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ReSharper.ForTea.Psi
{
	public interface IT4PsiModuleFactory
	{
		IT4PsiModule Produce(
			Lifetime lifetime,
			[NotNull] IPsiModules psiModules,
			[NotNull] DocumentManager documentManager,
			[NotNull] ChangeManager changeManager,
			[NotNull] IAssemblyFactory assemblyFactory,
			[NotNull] IShellLocks shellLocks,
			[NotNull] IProjectFile projectFile,
			[NotNull] T4FileDataCache fileDataCache,
			[NotNull] IT4Environment t4Environment,
			[NotNull] OutputAssemblies outputAssemblies
		);
	}
}
