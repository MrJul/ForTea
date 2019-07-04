using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.DEADCODE,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class UnsupportedLanguageHighlighting : T4HighlightingBase<IT4Token>
	{
		public UnsupportedLanguageHighlighting([NotNull] IT4Token associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "This language is not supported";
	}
}
