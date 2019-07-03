using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings {

	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class StatementAfterFeatureHighlighting : T4Highlighting<T4StatementBlock> {

		public override string ToolTip
			=> "A statement block cannot appear after a class feature block";

		/// <summary>Initializes a new instance of the <see cref="StatementAfterFeatureHighlighting"/> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public StatementAfterFeatureHighlighting([NotNull] T4StatementBlock associatedNode)
			: base(associatedNode) {
		}

	}

}
