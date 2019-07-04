using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings {

	/// <summary>Error highlighting for missing tokens.</summary>
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class MissingTokenHighlighting : T4HighlightingBase<MissingTokenErrorElement> {

		public override string ToolTip
			=> AssociatedNode.ErrorDescription;

		/// <summary>Initializes a new instance of the <see cref="MissingTokenHighlighting"/> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public MissingTokenHighlighting([NotNull] MissingTokenErrorElement associatedNode)
			: base(associatedNode) {
		}

	}

}
