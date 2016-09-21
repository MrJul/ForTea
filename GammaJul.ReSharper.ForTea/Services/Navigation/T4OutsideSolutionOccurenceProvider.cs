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
			var findResultText = findResult as FindResultText;
			if (findResultText == null || findResultText.DocumentRange.Document.GetOutsideSolutionPath().IsEmpty)
				return null;
			
			IRangeMarker rangeMarker = findResultText.Solution.GetComponent<DocumentManager>().CreateRangeMarker(findResultText.DocumentRange);
			return new T4OutsideSolutionOccurrence(rangeMarker);
		}

	}

}