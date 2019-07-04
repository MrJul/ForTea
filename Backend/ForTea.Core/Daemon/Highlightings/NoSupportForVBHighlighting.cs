using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings
{
	[StaticSeverityHighlighting(
		Severity.ERROR,
		CSharpProjectFileType.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class NoSupportForVBHighlighting : T4HighlightingBase<IT4Token>
	{
		public NoSupportForVBHighlighting([NotNull] IT4Token associatedNode) : base(associatedNode)
		{
		}

		public override string ToolTip => "Visual Basic is not supported";
	}
}
