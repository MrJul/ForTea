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
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	[QuickFix]
	public class ChangeAttributeValueQuickFix : QuickFixBase {
		private readonly InvalidAttributeValueHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			string text = _highlighting.AssociatedNode.GetText();
			DocumentRange range = _highlighting.AssociatedNode.GetDocumentRange();

			// create a hotspot around the directive value, with basic completion invoked
			return textControl =>
				Shell.Instance
					.GetComponent<LiveTemplatesManager>()
					.CreateHotspotSessionAtopExistingText(
						solution,
						DocumentOffset.InvalidOffset,
						textControl,
						LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
						HotspotHelper.CreateBasicCompletionHotspotInfo(text, range))
					.Execute();
		}

		public override string Text {
			get { return "Fix value"; }
		}

		public override bool IsAvailable(IUserDataHolder cache) {
			return _highlighting.IsValid() && _highlighting.DirectiveAttributeInfo != null;
		}

		public ChangeAttributeValueQuickFix([NotNull] InvalidAttributeValueHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}