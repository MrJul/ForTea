#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
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
				return new TextLookupRanges(range, range);
			}

			// completion has been triggered by a letter/number, determine which characters are before and after the caret
			// replace only those before in insert mode, replace before and after in replace mode
			TextRange nodeRange = node.GetDocumentRange().TextRange;
			var beforeCaretRange = new TextRange(nodeRange.StartOffset, caretStart);
			var afterCaretRange = new TextRange(caretStart, nodeRange.EndOffset);
			return new TextLookupRanges(beforeCaretRange, beforeCaretRange.Join(afterCaretRange));
		}

	}

}