using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Services.Navigation {

	/// <summary>Represents an occurence of text in an include file that is located outside of the solution.</summary>
	public class T4OutsideSolutionOccurrence : IOccurrence {

		[NotNull] private readonly IRangeMarker _rangeMarker;
		
		public bool IsValid
			=> _rangeMarker.IsValid;

		public OccurrenceType OccurrenceType
			=> OccurrenceType.TextualOccurrence;

		public ISolution GetSolution()
			=> null;

		public OccurrencePresentationOptions PresentationOptions { get; set; }
		
		public string DumpToString()
			=> _rangeMarker.DocumentRange.ToString();

		public bool Navigate(ISolution solution, PopupWindowContextSource windowContext, bool transferFocus, TabOptions tabOptions = TabOptions.Default) {
			if (!IsValid)
				return false;

			FileSystemPath path = _rangeMarker.Document.GetOutsideSolutionPath();
			if (path.IsEmpty)
				return false;

			var navigationInfo = new T4OutsideSolutionNavigationInfo(path, _rangeMarker.DocumentRange, transferFocus, tabOptions);
			var navigationOptions = NavigationOptions.FromWindowContext(windowContext, "Navigate to included file", transferFocus, tabOptions);
			var navigationManager = NavigationManager.GetInstance(solution);
			return navigationManager.Navigate<T4OutsideSolutionNavigationProvider, T4OutsideSolutionNavigationInfo>(navigationInfo, navigationOptions);
		}
		
		public T4OutsideSolutionOccurrence([NotNull] IRangeMarker rangeMarker) {
			_rangeMarker = rangeMarker;
		}

	}

}