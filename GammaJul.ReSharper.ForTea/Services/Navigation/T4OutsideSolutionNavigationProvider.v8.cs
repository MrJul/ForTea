using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Navigation.Navigation;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	public partial class T4OutsideSolutionNavigationProvider : INavigationProvider<T4OutsideSolutionNavigationInfo> {

		public bool IsApplicable(T4OutsideSolutionNavigationInfo data) {
			return data != null;
		}

		public IEnumerable<INavigationPoint> CreateNavigationPoints(T4OutsideSolutionNavigationInfo target) {
			return CreateNavigationPoints(target, EmptyList<INavigationPoint>.InstanceList);
		}

	}

}