using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>
	/// Error highlighting ignored assembly directives.
	/// </summary>
	[StaticSeverityHighlighting(Severity.WARNING, T4Language.Name, OverlapResolve = OverlapResolveKind.DEADCODE, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE)]
	public class IgnoredAssemblyDirectiveHighlighting : T4Highlighting<ITreeNode> {

		public override string ToolTip {
			get { return "Assembly directives are ignored in runtime templates. Use the assembly references of the project instead."; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IgnoredAssemblyDirectiveHighlighting"/> class.
		/// </summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public IgnoredAssemblyDirectiveHighlighting([NotNull] ITreeNode associatedNode)
			: base(associatedNode) {
		}

	}

}