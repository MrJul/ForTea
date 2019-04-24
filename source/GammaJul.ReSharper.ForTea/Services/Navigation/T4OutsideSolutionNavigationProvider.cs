using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationProviders;
using JetBrains.ReSharper.Features.Navigation.Core.Navigation;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	[NavigationProvider]
	public class T4OutsideSolutionNavigationProvider : INavigationProvider<T4OutsideSolutionNavigationInfo> {

		[NotNull] private readonly FileSystemPathNavigator _fileSystemPathNavigator;
		
		public bool IsApplicable(T4OutsideSolutionNavigationInfo data)
			=> data != null;

		public IEnumerable<INavigationPoint> CreateNavigationPoints(T4OutsideSolutionNavigationInfo target)
			=> new INavigationPoint[] {
				_fileSystemPathNavigator.CreateNavigationPoint(target.FileSystemPath, target.DocumentRange.TextRange, "T4", "T4")
			};

		public T4OutsideSolutionNavigationProvider([NotNull] FileSystemPathNavigator fileSystemPathNavigator) {
			_fileSystemPathNavigator = fileSystemPathNavigator;
		}

	}

}