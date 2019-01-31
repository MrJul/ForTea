using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class MissingRequiredAttributeHighlighting : T4Highlighting<IT4Token> {

		[NotNull]
		public string MissingAttributeName { get; }

		public override string ToolTip
			=> "Missing required attribute \"" + MissingAttributeName + "\"";

		public MissingRequiredAttributeHighlighting([NotNull] IT4Token directiveNameNode, [NotNull] string missingAttributeName)
			: base(directiveNameNode) {
			MissingAttributeName = missingAttributeName;
		}

	}

}