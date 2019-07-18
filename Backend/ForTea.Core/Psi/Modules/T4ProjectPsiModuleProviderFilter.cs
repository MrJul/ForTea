using System;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Psi.Modules {

	/// <summary>Provides a <see cref="T4ProjectPsiModuleHandler"/> for a given project.</summary>
	[SolutionComponent]
	class T4ProjectPsiModuleProviderFilter : IProjectPsiModuleProviderFilter {

		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly IT4MacroResolver _resolver;
		[NotNull] private readonly PsiProjectFileTypeCoordinator _coordinator;

		public Tuple<IProjectPsiModuleHandler, IPsiModuleDecorator> OverrideHandler(
			Lifetime lifetime,
			IProject project,
			IProjectPsiModuleHandler handler
		) {
			var t4ModuleHandler = new T4ProjectPsiModuleHandler(lifetime,
				handler,
				_changeManager,
				_t4Environment,
				project,
				_resolver,
				_coordinator
			);
			return new Tuple<IProjectPsiModuleHandler, IPsiModuleDecorator>(t4ModuleHandler, null);
		}

		public T4ProjectPsiModuleProviderFilter(
			[NotNull] ChangeManager changeManager,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IT4MacroResolver resolver, [NotNull] PsiProjectFileTypeCoordinator coordinator) {
			_changeManager = changeManager;
			_t4Environment = t4Environment;
			_resolver = resolver;
			_coordinator = coordinator;
		}

	}

}