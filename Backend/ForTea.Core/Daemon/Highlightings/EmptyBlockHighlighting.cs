using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.WARNING,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE
	)]
	public class EmptyBlockHighlighting : T4HighlightingBase<IT4CodeBlock>
	{
		public EmptyBlockHighlighting([NotNull] IT4CodeBlock associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Empty block";
	}
}
