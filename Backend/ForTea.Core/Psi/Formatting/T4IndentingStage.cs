using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	public class T4IndentingStage : IndentingStage<CodeFormattingContext, T4FormatterSettingsKey>
	{
		public T4IndentingStage(
			CodeFormattingContext context,
			FmtSettings<T4FormatterSettingsKey> settings,
			IFormatterInfoProvider<CodeFormattingContext, T4FormatterSettingsKey> provider,
			IProgressIndicator progress,
			SequentialNodeIterator<CodeFormattingContext, T4FormatterSettingsKey> iterator)
			: base(context, settings, provider, progress, iterator)
		{
		}

		public Whitespace? CalcCustomIndent() => Whitespace.Empty;
	}
}
