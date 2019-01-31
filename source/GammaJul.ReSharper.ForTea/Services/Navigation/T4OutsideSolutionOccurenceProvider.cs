using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi.Search;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	/// <summary>
	/// An implementation of <see cref="IOccurrenceProvider" /> that creates occurences for find results that are
	/// inside included files that aren't present in the current solution, and thus ignored by ReSharper.
	/// </summary>
	[OccurrenceProvider(Priority = 10000)]
	public class T4OutsideSolutionOccurrenceProvider : IOccurrenceProvider {

		public IOccurrence MakeOccurrence(FindResult findResult) {
			if (!(findResult is FindResultText findResultText)
			|| findResultText.DocumentRange.Document.GetOutsideSolutionPath().IsEmpty)
				return null;
			
			IRangeMarker rangeMarker = findResultText.Solution.GetComponent<DocumentManager>().CreateRangeMarker(findResultText.DocumentRange);
			return new T4OutsideSolutionOccurrence(rangeMarker);
		}

	}

}