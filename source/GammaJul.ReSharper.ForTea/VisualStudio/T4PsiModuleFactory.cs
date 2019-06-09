using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ReSharper.ForTea.VisualStudio
{
	[ShellComponent]
	internal sealed class T4PsiModuleFactory : IT4PsiModuleFactory
	{
		public IT4PsiModule Produce(
			Lifetime lifetime,
			IPsiModules psiModules,
			DocumentManager documentManager,
			ChangeManager changeManager,
			IAssemblyFactory assemblyFactory,
			IShellLocks shellLocks,
			IProjectFile projectFile,
			T4FileDataCache fileDataCache,
			IT4Environment t4Environment,
			OutputAssemblies outputAssemblies
		) => new T4PsiModule(
			lifetime,
			psiModules,
			documentManager,
			changeManager,
			assemblyFactory,
			shellLocks,
			projectFile,
			fileDataCache,
			t4Environment,
			outputAssemblies
		);
	}
}
