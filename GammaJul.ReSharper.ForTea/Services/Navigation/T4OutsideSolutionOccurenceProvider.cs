using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Feature.Services.Search;
using JetBrains.ReSharper.Psi.Search;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	/// <summary>
	/// An implementation of <see cref="IOccurenceProvider" /> that creates occurences for find results that are
	/// inside included files that aren't present in the current solution, and thus ignored by ReSharper.
	/// </summary>
	[OccurenceProvider(Priority = 10000)]
	public class T4OutsideSolutionOccurenceProvider : IOccurenceProvider {

		public IOccurence MakeOccurence(FindResult findResult) {
			var findResultText = findResult as FindResultText;
			if (findResultText == null
			|| findResultText.DocumentRange.Document.GetOutsideSolutionPath().IsEmpty)
				return null;

			IRangeMarker rangeMarker = findResultText.Solution.GetComponent<DocumentManager>().CreateRangeMarker(findResultText.DocumentRange);
			return new T4OutsideSolutionOccurence(rangeMarker);
		}

	}

}