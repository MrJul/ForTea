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
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>
	/// Error highlighting nodes after the last feature.
	/// </summary>
	[StaticSeverityHighlighting(Severity.ERROR, T4Language.Name, OverlapResolve = OverlapResolveKind.DEADCODE, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE)]
	public class AfterLastFeatureHighlighting : T4Highlighting<ITreeNode> {

		public override string ToolTip {
			get { return "A template containing a class feature must end with a class feature"; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AfterLastFeatureHighlighting"/> class.
		/// </summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public AfterLastFeatureHighlighting([NotNull] ITreeNode associatedNode)
			: base(associatedNode) {
		}

	}
}