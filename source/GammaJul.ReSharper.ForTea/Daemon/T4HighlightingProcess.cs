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
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Process that highlights block tags and missing token errors.
	/// </summary>
	internal sealed class T4HighlightingProcess : T4DaemonStageProcess {
		
		/// <summary>
		/// Processes a node, before its descendants are processed.
		/// </summary>
		/// <param name="element">The node to process.</param>
		public override void ProcessBeforeInterior(ITreeNode element) {
			string attributeId = GetHighlightingAttributeId(element);
			if (attributeId != null) {
				DocumentRange range = element.GetHighlightingRange();
				AddHighlighting(new HighlightingInfo(range, new PredefinedHighlighting(attributeId, range)));
			}
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			var tokenType = element.GetTokenType() as T4TokenNodeType;
			if (tokenType == null)
				return null;
			
			if (tokenType.IsTag)
				return VsPredefinedHighlighterIds.HtmlServerSideScript;
			if (tokenType == T4TokenNodeTypes.Equal)
				return VsPredefinedHighlighterIds.HtmlOperator;
			if (tokenType == T4TokenNodeTypes.Quote || tokenType == T4TokenNodeTypes.Value)
				return VsPredefinedHighlighterIds.HtmlAttributeValue;
			if (tokenType == T4TokenNodeTypes.Name) {
				if (element.Parent is IT4Directive)
					return VsPredefinedHighlighterIds.HtmlElementName;
				return VsPredefinedHighlighterIds.HtmlAttributeName;
			}
			return null;
		}
		
		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
			: base(file, daemonProcess) {
		}

	}

}