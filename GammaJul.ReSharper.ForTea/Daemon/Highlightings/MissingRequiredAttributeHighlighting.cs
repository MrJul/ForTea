using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(Severity.ERROR, T4Language.Name, OverlapResolve = OverlapResolveKind.ERROR, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE)]
	public class MissingRequiredAttributeHighlighting : T4Highlighting<IT4Token> {
		private readonly string _missingAttributeName;

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