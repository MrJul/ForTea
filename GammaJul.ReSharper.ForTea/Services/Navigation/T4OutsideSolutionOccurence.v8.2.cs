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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Search;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	/// <summary>
	/// Represents an occurence of text in an include file that is located outside of the solution.
	/// </summary>
	public partial class T4OutsideSolutionOccurence : IOccurence {

		private readonly List<IOccurence> _mergedItems = new List<IOccurence>();

		public TextRange TextRange {
			get { return _rangeMarker.Range; }
		}
		
		public OccurenceType OccurenceType {
			get { return OccurenceType.TextualOccurence; }
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
		
		public object MergeKey {
			get {
				// TODO: provide a real merge key
				return this;
			}
		}

		public IList<IOccurence> MergedItems {
			get { return _mergedItems; }
		}

		public ProjectModelElementEnvoy ProjectModelElementEnvoy {
			get { return ProjectModelElementEnvoy.Empty; }
		}

	}

}