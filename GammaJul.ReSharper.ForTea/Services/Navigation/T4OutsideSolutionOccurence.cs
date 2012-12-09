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
	public class T4OutsideSolutionOccurence : IOccurence {

		private readonly List<IOccurence> _mergedItems = new List<IOccurence>();
		private readonly IRangeMarker _rangeMarker;

		public TextRange TextRange {
			get { return _rangeMarker.Range; }
		}

		public ProjectModelElementEnvoy ProjectModelElement {
			get { return ProjectModelElementEnvoy.Empty; }
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
			return NavigationManager.GetInstance(solution).Navigate(navigationInfo, navigationOptions);
		}

		public T4OutsideSolutionOccurence([NotNull] IRangeMarker rangeMarker) {
			_rangeMarker = rangeMarker;
		}

	}

}