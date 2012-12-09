using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Provides a <see cref="T4ProjectPsiModuleHandler"/> for a given project.
	/// </summary>
	[SolutionComponent]
	public class T4ProjectPsiModuleProviderFilter : IProjectPsiModuleProviderFilter {
		private readonly T4PsiModuleProvider _t4PsiModuleProvider;

		public JetTuple<IProjectPsiModuleHandler, IPsiModuleDecorator> OverrideHandler(Lifetime lifetime, IProject project, IProjectPsiModuleHandler handler) {
			var t4ModuleHandler = new T4ProjectPsiModuleHandler(handler, _t4PsiModuleProvider);
			return new JetTuple<IProjectPsiModuleHandler, IPsiModuleDecorator>(t4ModuleHandler, null);
		}

		public T4ProjectPsiModuleProviderFilter([NotNull] T4PsiModuleProvider t4PsiModuleProvider) {
			_t4PsiModuleProvider = t4PsiModuleProvider;
		}

	}

}