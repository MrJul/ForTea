using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	public partial class T4OutsideSolutionNavigationProvider : INavigationProvider {

		public double Priority {
			get { return 10000.0; }
		}

		public IEnumerable<Type> GetSupportedTargetTypes() {
			return new[] {
				typeof(T4OutsideSolutionNavigationInfo)
			};
		}

		public IEnumerable<INavigationPoint> CreateNavigationPoints(object target, IEnumerable<INavigationPoint> basePoints) {
			var typedTarget = target as T4OutsideSolutionNavigationInfo;
			return typedTarget != null ? CreateNavigationPoints(typedTarget, basePoints) : basePoints;
		}

	}

}