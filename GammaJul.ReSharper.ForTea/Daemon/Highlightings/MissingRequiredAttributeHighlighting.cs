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
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(Severity.ERROR, T4Language.Name, OverlapResolve = OverlapResolveKind.ERROR, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE)]
	public class MissingRequiredAttributeHighlighting : T4Highlighting<IT4Token> {

		[NotNull] private readonly string _missingAttributeName;

		[NotNull]
		public string MissingAttributeName {
			get { return _missingAttributeName; }
		}

		public override string ToolTip {
			get { return "Missing required attribute \"" + _missingAttributeName + "\""; }
		}

		public MissingRequiredAttributeHighlighting([NotNull] IT4Token directiveNameNode, [NotNull] string missingAttributeName)
			: base(directiveNameNode) {
			_missingAttributeName = missingAttributeName;
		}

	}

}