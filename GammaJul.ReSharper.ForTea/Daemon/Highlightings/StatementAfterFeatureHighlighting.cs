using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(Severity.ERROR, T4Language.Name, OverlapResolve = OverlapResolveKind.DEADCODE, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE)]
	public class StatementAfterFeatureHighlighting : T4Highlighting<T4StatementBlock> {

		public override string ToolTip {
			get { return "A statement block cannot appear after a class feature block"; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StatementAfterFeatureHighlighting"/> class.
		/// </summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public StatementAfterFeatureHighlighting([NotNull] T4StatementBlock associatedNode)
			: base(associatedNode) {
		}

	}

}