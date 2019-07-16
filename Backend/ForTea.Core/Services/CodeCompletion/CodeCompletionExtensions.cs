using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	/// <summary>Contains extension methods for code completion.</summary>
	public static class CodeCompletionExtensions {
		
		[NotNull]
		public static TextLookupRanges GetRanges([NotNull] this CodeCompletionContext context, [NotNull] ITreeNode node) {
			TokenNodeType tokenType = node.GetTokenType();

			// completion has been triggered by space or quote, insert/replace at the caret (just after the space/quote)
			if (tokenType == T4TokenNodeTypes.WHITE_SPACE || tokenType == T4TokenNodeTypes.QUOTE) {
				var range = new DocumentRange(context.CaretDocumentOffset, context.CaretDocumentOffset);
				return new TextLookupRanges(range, range);
			}

			// completion has been triggered by a letter/number, determine which characters are before and after the caret
			// replace only those before in insert mode, replace before and after in replace mode
			TextRange nodeRange = node.GetDocumentRange().TextRange;
			var beforeCaretRange = new DocumentRange(new DocumentOffset(context.Document, nodeRange.StartOffset), context.CaretDocumentOffset);
			var afterCaretRange = new DocumentRange(context.CaretDocumentOffset, new DocumentOffset(context.Document, nodeRange.EndOffset));
			return new TextLookupRanges(beforeCaretRange, beforeCaretRange.Join(afterCaretRange));
		}

	}

}