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
using System.Linq;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	// TODO: remove this class, it's not used for R# 8 since we're using the new NavigateablePsiSourceFileWithLocation
	// with built-in navigation support rather than a custom ISourceFile.
	/// <summary>
	/// An implementation of <see cref="IOccurenceProvider" /> that creates occurences for find results that are
	/// inside included files that aren't present in the current solution, and thus ignored by ReSharper.
	/// </summary>
	[OccurenceProvider(Priority = 10000)]
	public class T4OutsideSolutionOccurenceProvider : IOccurenceProvider {

		public IOccurence MakeOccurence(FindResult findResult) {
			var findResultText = findResult as FindResultText;
			if (findResultText == null || findResultText.DocumentRange.Document.GetOutsideSolutionPath().IsEmpty)
				return null;
			
			IRangeMarker rangeMarker = findResultText.Solution.GetComponent<DocumentManager>().CreateRangeMarker(findResultText.DocumentRange);
			return new T4OutsideSolutionOccurence(rangeMarker);
		}

	}

}