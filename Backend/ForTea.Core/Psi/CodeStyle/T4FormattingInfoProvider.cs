using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.CodeStyle
{
	[Language(typeof(T4Language))]
	public class
		T4FormattingInfoProvider : FormatterInfoProviderWithFluentApi<CodeFormattingContext, T4FormatterSettingsKey>
	{
		public T4FormattingInfoProvider(ISettingsSchema settingsSchema) : base(settingsSchema)
		{
		}

		public override IndentingStage<CodeFormattingContext, T4FormatterSettingsKey> CreateIndentingStage(
			CodeFormattingContext context,
			FmtSettings<T4FormatterSettingsKey> settings,
			IProgressIndicator progress,
			SequentialNodeIterator<CodeFormattingContext, T4FormatterSettingsKey> iterator = null)
		{
			TryFinishAddingRules();
			return new T4IndentingStage(
				context,
				settings,
				this,
				progress,
				iterator);
		}

		public override void ModifyIndent(
			ITreeNode nodeToIndent, ref Whitespace? indent, CodeFormattingContext context,
			IIndentingStage<T4FormatterSettingsKey> callback, IndentType indentType)
		{
			var t4IndentingStage = callback as T4IndentingStage;
			indent = t4IndentingStage.NotNull("wrong callback").CalcCustomIndent(
				nodeToIndent,
				indent,
				callback.Settings.Settings.GetIndentWhitespace(),
				indentType
			);

			base.ModifyIndent(nodeToIndent,
				ref indent,
				context,
				callback,
				indentType);
		}

		public override ProjectFileType MainProjectFileType => T4ProjectFileType.Instance;
	}
}
