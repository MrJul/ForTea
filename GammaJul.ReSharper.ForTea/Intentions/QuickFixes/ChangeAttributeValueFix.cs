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