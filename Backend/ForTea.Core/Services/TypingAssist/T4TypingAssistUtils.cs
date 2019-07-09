using JetBrains.TextControl;

namespace GammaJul.ForTea.Core.Services.TypingAssist
{
	public static class T4TypingAssistUtils
	{
		public static int GetOffset(this ITextControl textControl) =>
			textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
	}
}
