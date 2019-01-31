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
using GammaJul.ReSharper.ForTea.Psi;
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
using JetBrains.ReSharper.Psi.CachingLexers;
using JetBrains.TextControl;
using JetBrains.TextControl.Util;

namespace GammaJul.ReSharper.ForTea.Services.TypingAssist {

	[SolutionComponent]
	public class T4TypingAssist : TypingAssistLanguageBase<T4Language>, ITypingHandler {

		private readonly ICodeCompletionSessionManager _codeCompletionSessionManager;

		protected override bool IsSupported(ITextControl textControl) {
			IPsiSourceFile psiSourceFile = textControl.Document.GetPsiSourceFile(Solution);
			return psiSourceFile != null
				&& psiSourceFile.LanguageType.Is<T4ProjectFileType>()
				&& psiSourceFile.IsValid();
		}

		public bool QuickCheckAvailability(ITextControl textControl, IPsiSourceFile projectFile)
			=> projectFile.LanguageType.Is<T4ProjectFileType>();

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
				SkippingTypingAssist.SetCharsToSkip(textControl.Document, "\"");

				// popup auto completion
				_codeCompletionSessionManager.ExecuteAutoCompletion<T4AutopopupSettingsKey>(textControl, Solution, key => key.InDirectives);
			});

			return true;
		}

		/// <summary>
		/// When a " is typed, insert another ".
		/// </summary>
		private bool OnQuoteTyped(ITypingContext context) {
			ITextControl textControl = context.TextControl;

			// the " character should be skipped to avoid double insertions
			if (SkippingTypingAssist.ShouldSkip(textControl.Document, context.Char)) {
				SkippingTypingAssist.SkipIfNeeded(textControl.Document, context.Char);
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
				SkippingTypingAssist.SetCharsToSkip(textControl.Document, "\"");

				// popup auto completion
				_codeCompletionSessionManager.ExecuteAutoCompletion<T4AutopopupSettingsKey>(textControl, Solution, key => key.InDirectives);
			});

			return true;
		}

		public T4TypingAssist(
			[NotNull] Lifetime lifetime,
			[NotNull] ISolution solution,
			[NotNull] ISettingsStore settingsStore,
			[NotNull] CachingLexerService cachingLexerService,
			[NotNull] ICommandProcessor commandProcessor,
			[NotNull] IPsiServices psiServices,
			[NotNull] IExternalIntellisenseHost externalIntellisenseHost,
			[NotNull] SkippingTypingAssist skippingTypingAssist,
			[NotNull] ITypingAssistManager typingAssistManager,
			[NotNull] ICodeCompletionSessionManager codeCompletionSessionManager)
			: base(solution, settingsStore, cachingLexerService, commandProcessor, psiServices, externalIntellisenseHost, skippingTypingAssist) {
			
			_codeCompletionSessionManager = codeCompletionSessionManager;

			typingAssistManager.AddTypingHandler(lifetime, '=', this, OnEqualTyped, IsTypingSmartParenthesisHandlerAvailable);
			typingAssistManager.AddTypingHandler(lifetime, '"', this, OnQuoteTyped, IsTypingSmartParenthesisHandlerAvailable);
		}

	}

}