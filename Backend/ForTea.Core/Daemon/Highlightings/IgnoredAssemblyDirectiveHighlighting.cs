using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Highlightings {

	/// <summary>Error highlighting ignored assembly directives.</summary>
	[StaticSeverityHighlighting(
		Severity.WARNING,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE
	)]
	public class IgnoredAssemblyDirectiveHighlighting : T4HighlightingBase<ITreeNode> {

		/// <summary>Initializes a new instance of the <see cref="IgnoredAssemblyDirectiveHighlighting" /> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		public IgnoredAssemblyDirectiveHighlighting([NotNull] ITreeNode associatedNode)
			: base(associatedNode) {
		}

		public override string ToolTip
			=> "Assembly directives are ignored in runtime templates. Use the assembly references of the project instead.";

	}

}