#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

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