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
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Feature.Services.Search;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.PopupWindowManager;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	/// <summary>
	/// Represents an occurence of text in an include file that is located outside of the solution.
	/// </summary>
	public partial class T4OutsideSolutionOccurence : IOccurence {

		private readonly List<IOccurence> _mergedItems = new List<IOccurence>();
		private readonly IRangeMarker _rangeMarker;

		public TextRange TextRange {
			get { return _rangeMarker.Range; }
		}

		public DeclaredElementEnvoy<ITypeMember> TypeMember {
			get { return null; }
		}

		public DeclaredElementEnvoy<ITypeElement> TypeElement {
			get { return null; }
		}

		public DeclaredElementEnvoy<INamespace> Namespace {
			get { return null; }
		}

		public OccurenceType OccurenceType {
			get { return OccurenceType.TextualOccurence; }
		}

		public bool IsValid {
			get { return _rangeMarker.IsValid; }
		}

		public object MergeKey {
			get {
				// TODO: provide a real merge key
				return this;
			}
		}

		public IList<IOccurence> MergedItems {
			get { return _mergedItems; }
		}

		public OccurencePresentationOptions PresentationOptions { get; set; }

		public string DumpToString() {
			return _rangeMarker.DocumentRange.ToString();
		}

		public bool Navigate(ISolution solution, PopupWindowContextSource windowContext, bool transferFocus, TabOptions tabOptions = TabOptions.Default) {
			if (!IsValid)
				return false;

			FileSystemPath path = _rangeMarker.Document.GetOutsideSolutionPath();
			if (path.IsEmpty)
				return false;

			var navigationInfo = new T4OutsideSolutionNavigationInfo(path, _rangeMarker.Range, transferFocus, tabOptions);
			NavigationOptions navigationOptions = NavigationOptions.FromWindowContext(windowContext, "Navigate to included file", transferFocus, tabOptions);
			NavigationManager navigationManager = NavigationManager.GetInstance(solution);
			return NavigateCore(navigationManager, navigationInfo, navigationOptions);
		}

		public T4OutsideSolutionOccurence([NotNull] IRangeMarker rangeMarker) {
			_rangeMarker = rangeMarker;
		}

	}

}