using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Highlightings {

	/// <summary>Error highlighting nodes after the last feature.</summary>
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class AfterLastFeatureHighlighting : T4HighlightingBase<ITreeNode> {

		/// <summary>Initializes a new instance of the <see cref="AfterLastFeatureHighlighting" /> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public AfterLastFeatureHighlighting([NotNull] ITreeNode associatedNode)
			: base(associatedNode) {
		}

		public override string ToolTip
			=> "A template containing a class feature must end with a class feature";

	}

}