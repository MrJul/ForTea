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
using System;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.TextControl;
using JetBrains.Util;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	[QuickFix]
	public class ChangeAttributeValueFix : QuickFixBase {
		private readonly InvalidAttributeValueHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			string text = _highlighting.AssociatedNode.GetText();
			TextRange range = _highlighting.AssociatedNode.GetDocumentRange().TextRange;

			// create a hotspot around the directive value, with basic completion invoked
			return textControl =>
				Shell.Instance
					.GetComponent<LiveTemplatesManager>()
					.CreateHotspotSessionAtopExistingText(
						solution,
						TextRange.InvalidRange,
						textControl,
						LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
						new HotspotInfo(
							new TemplateField(text, new MacroCallExpression(new BasicCompletionMacro()), 0),
							range))
					.Execute();
		}

		public override string Text {
			get { return "Fix value"; }
		}

		public override bool IsAvailable(IUserDataHolder cache) {
			return _highlighting.IsValid() && _highlighting.DirectiveAttributeInfo != null;
		}

		public ChangeAttributeValueFix([NotNull] InvalidAttributeValueHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}