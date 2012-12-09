using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.CodeStyle;
using GammaJul.ReSharper.ForTea.Services.CodeCompletion;
using JetBrains.Annotations;
using JetBrains.Application.CommandProcessing;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.TypingAssist;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.TextControl;
using JetBrains.TextControl.Util;

namespace GammaJul.ReSharper.ForTea.Services.TypingAssist {

	[SolutionComponent]
	public class T4TypingAssist : TypingAssistLanguageBase<T4Language, T4CodeFormatter>, ITypingHandler {
		private readonly SkippingTypingAssist _skippingTypingAssist;
		private readonly IntellisenseManager _intellisenseManager;

		protected override bool IsSupported(ITextControl textControl) {
			IPsiSourceFile psiSourceFile = textControl.Document.GetPsiSourceFile(Solution);
			return psiSourceFile != null
				&& psiSourceFile.LanguageType.Is<T4ProjectFileType>()
				&& psiSourceFile.IsValid();
		}

		public bool QuickCheckAvailability(ITextControl textControl, IPsiSourceFile projectFile) {
			return projectFile.LanguageType.Is<T4ProjectFileType>();
		}

		/// <summary>
		/// When = is typed, insert "".
		/// </summary>
		private bool OnEqualTyped(ITypingContext context) {
			ITextControl textControl = context.TextControl;

			// get the token type before =
			CachingLexer cachingLexer = GetCachingLexer(textControl);
			int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
			if (cachingLexer == null || offset <= 0 || !cachingLexer.FindTokenAt(offset - 1))
				return false;

			// do nothing if we're not after an attribute name
			TokenNodeType tokenType = cachingLexer.TokenType;
			if (tokenType != T4TokenNodeTypes.Name)
				return false;

			// insert =
			TextControlUtil.DeleteSelection(textControl);
			textControl.FillVirtualSpaceUntilCaret();
			textControl.Document.InsertText(offset, "=");
			textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);

			// insert ""
			context.QueueCommand(() => {
				using (CommandProcessor.UsingCommand("Inserting \"\"")) {
					textControl.Document.InsertText(offset + 1, "\"\"");
					textControl.Caret.MoveTo(offset + 2, CaretVisualPlacement.DontScrollIfVisible);
				}

				// ignore if a subsequent " is typed by the user
				_skippingTypingAssist.SetCharsToSkip(textControl.Document, "\"");

				// popup auto completion
				_intellisenseManager.ExecuteAutoCompletion<T4AutopopupSettingsKey>(textControl, Solution, key => key.InDirectives);
			});

			return true;
		}

		/// <summary>
		/// When a " is typed, insert another ".
		/// </summary>
		private bool OnQuoteTyped(ITypingContext context) {
			ITextControl textControl = context.TextControl;

			// the " character should be skipped to avoid double insertions
			if (_skippingTypingAssist.ShouldSkip(textControl.Document, context.Char)) {
				_skippingTypingAssist.SkipIfNeeded(textControl.Document, context.Char);
				return true;
			}

			// get the token type after "
			CachingLexer cachingLexer = GetCachingLexer(textControl);
			int offset = textControl.Selection.OneDocRangeWithCaret().GetMinOffset();
			if (cachingLexer == null || offset <= 0 || !cachingLexer.FindTokenAt(offset))
				return false;

			// there is already another quote after the ", swallow the typing
			TokenNodeType tokenType = cachingLexer.TokenType;
			if (tokenType == T4TokenNodeTypes.Quote) {
				textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
				return true;
			}

			// we're inside or after an attribute value, simply do nothing and let the " be typed
			if (tokenType == T4TokenNodeTypes.Value)
				return false;

			// insert the first "
			TextControlUtil.DeleteSelection(textControl);
			textControl.FillVirtualSpaceUntilCaret();
			textControl.Document.InsertText(offset, "\"");

			// insert the second "
			context.QueueCommand(() => {
				using (CommandProcessor.UsingCommand("Inserting \"")) {
					textControl.Document.InsertText(offset + 1, "\"");
					textControl.Caret.MoveTo(offset + 1, CaretVisualPlacement.DontScrollIfVisible);
				}

				// ignore if a subsequent " is typed by the user
				_skippingTypingAssist.SetCharsToSkip(textControl.Document, "\"");

				// popup auto completion
				_intellisenseManager.ExecuteAutoCompletion<T4AutopopupSettingsKey>(textControl, Solution, key => key.InDirectives);
			});

			return true;
		}

		public T4TypingAssist([NotNull] Lifetime lifetime, [NotNull] ISolution solution, [NotNull] ISettingsStore settingsStore,
			[NotNull] CachingLexerService cachingLexerService, [NotNull] ICommandProcessor commandProcessor, [NotNull] IPsiServices psiServices,
			[NotNull] ITypingAssistManager typingAssistManager, [NotNull] SkippingTypingAssist skippingTypingAssist, [NotNull] IntellisenseManager intellisenseManager)
			: base(solution, settingsStore, cachingLexerService, commandProcessor, psiServices) {
			
			_skippingTypingAssist = skippingTypingAssist;
			_intellisenseManager = intellisenseManager;

			typingAssistManager.AddTypingHandler(lifetime, '=', this, OnEqualTyped, IsTypingSmartParenthesisHandlerAvailable);
			typingAssistManager.AddTypingHandler(lifetime, '"', this, OnQuoteTyped, IsTypingSmartParenthesisHandlerAvailable);
		}

	}

}