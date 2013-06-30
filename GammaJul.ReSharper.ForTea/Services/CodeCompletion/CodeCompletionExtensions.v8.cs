using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	public partial class CodeCompletionExtensions {

		[NotNull]
		private static TextLookupRanges CreateRanges(TextRange insertRange, TextRange replaceRange) {
			return new TextLookupRanges(insertRange, replaceRange);
		}

	}

}