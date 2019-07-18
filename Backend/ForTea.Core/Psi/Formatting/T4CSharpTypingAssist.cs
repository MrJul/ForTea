using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.CommandProcessing;
using JetBrains.Application.Settings;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CSharp.TypingAssist;
using JetBrains.ReSharper.Feature.Services.TypingAssist;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CachingLexers;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.TextControl;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[SolutionComponent]
	public sealed class T4CSharpTypingAssist : CSharpTypingAssistBase
	{
		public T4CSharpTypingAssist(Lifetime lifetime,
			[NotNull] ISolution solution,
			[NotNull] ICommandProcessor commandProcessor,
			[NotNull] SkippingTypingAssist skippingTypingAssist,
			[NotNull] CachingLexerService cachingLexerService,
			[NotNull] ISettingsStore settingsStore,
			[NotNull] ITypingAssistManager typingAssistManager,
			[NotNull] IPsiServices psiServices,
			[NotNull] IExternalIntellisenseHost externalIntellisenseHost
		) : base(
			lifetime,
			solution,
			commandProcessor,
			skippingTypingAssist,
			cachingLexerService,
			settingsStore,
			typingAssistManager,
			psiServices,
			externalIntellisenseHost
		)
		{
		}

		protected override bool IsSupported(ITextControl textControl) => false;

		public override bool QuickCheckAvailability(ITextControl textControl, IPsiSourceFile projectFile) =>
		    projectFile.LanguageType.Is<T4ProjectFileType>();

		protected override bool IsStopperTokenForStringLiteral(TokenNodeType tokenType)
		{
			if (tokenType is T4TokenNodeType t4Token && t4Token == T4TokenNodeTypes.BLOCK_END)
				return true;
			return base.IsStopperTokenForStringLiteral(tokenType);
		}

		protected override string GetLineTextBeforeOffset(ITextControl textControl, int lexerOffset)
		{
			return "          ";
			var indent = base.GetLineTextBeforeOffset(textControl, lexerOffset);
			return GetCodeBehindIndent(this, textControl, lexerOffset, indent);
		}
	}
}
