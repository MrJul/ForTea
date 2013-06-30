using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	public partial class T4OutsideSolutionOccurence {
		
		public ProjectModelElementEnvoy ProjectModelElement {
			get { return ProjectModelElementEnvoy.Empty; }
		}
		
		private static bool NavigateCore([NotNull] NavigationManager manager, [NotNull] T4OutsideSolutionNavigationInfo info, [NotNull] NavigationOptions options) {
			return manager.Navigate(info, options);
		}

	}

}