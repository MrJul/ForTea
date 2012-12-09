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