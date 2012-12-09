using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	/// <summary>
	/// Contains extension methods for code completion.
	/// </summary>
	public static class CodeCompletionExtensions {
		
		[NotNull]
		public static TextLookupRanges GetRanges([NotNull] this CodeCompletionContext context, [NotNull] ITreeNode node) {
			int caretStart = context.CaretDocumentRange.TextRange.StartOffset;
			TokenNodeType tokenType = node.GetTokenType();

			// completion has been triggered by space or quote, insert/replace at the caret (just after the space/quote)
			if (tokenType == T4TokenNodeTypes.Space || tokenType == T4TokenNodeTypes.Quote) {
				var range = new TextRange(caretStart, caretStart);
				return new TextLookupRanges(range, false, range);
			}

			// completion has been triggered by a letter/number, determine which characters are before and after the caret
			// replace only those before in insert mode, replace before and after in replace mode
			TextRange nodeRange = node.GetDocumentRange().TextRange;
			var beforeCaretRange = new TextRange(nodeRange.StartOffset, caretStart);
			var afterCaretRange = new TextRange(caretStart, nodeRange.EndOffset);
			return new TextLookupRanges(beforeCaretRange, false, beforeCaretRange.Join(afterCaretRange));
		}

	}

}