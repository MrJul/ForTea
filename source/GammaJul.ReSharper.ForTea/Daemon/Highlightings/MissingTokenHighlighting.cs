using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>Error highlighting for missing tokens.</summary>
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class MissingTokenHighlighting : T4Highlighting<MissingTokenErrorElement> {

		public override string ToolTip
			=> AssociatedNode.ErrorDescription;

		/// <summary>Initializes a new instance of the <see cref="MissingTokenHighlighting"/> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public MissingTokenHighlighting([NotNull] MissingTokenErrorElement associatedNode)
			: base(associatedNode) {
		}

	}

}