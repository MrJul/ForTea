using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Application.CommandProcessing;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.TypingAssist;
using JetBrains.ReSharper.Feature.Services.TypingAssist;
using JetBrains.ReSharper.Feature.Services.Web.TypingAssist;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.TextControl;

namespace GammaJul.ReSharper.ForTea.Services.TypingAssist {

	/// <summary>
	/// Typing assistant for C# embedded in T4 files.
	/// </summary>
	[SolutionComponent]
	public class T4CSharpTypingAssist : CSharpTypingAssistBase {

		/// <summary>
		/// Gets whether a given text control is supported by the assistant.
		/// </summary>
		/// <param name="textControl">The text control to check.</param>
		/// <returns><c>true</c> if <paramref name="textControl"/> is a T4 file with C# code behind, <c>false</c> otherwise.</returns>
		protected override bool IsSupported(ITextControl textControl) {
			return WebTypingAssistUtil.IsProjectFileSupported<T4ProjectFileType, CSharpLanguage>(textControl, Solution);
		}

		/// <summary>
		/// Returns the offset difference between a text control and a lexer.
		/// </summary>
		/// <param name="textControl">The text control.</param>
		/// <param name="offset">The original offset.</param>
		/// <returns>Always <paramref name="offset"/>.</returns>
		public override int TextControlToLexer(ITextControl textControl, int offset) {
			return offset;
		}

		/// <summary>
		/// Returns the offset difference between a lexer and a text control.
		/// </summary>
		/// <param name="textControl">The text control.</param>
		/// <param name="offset">The original offset.</param>
		/// <returns>Always <paramref name="offset"/>.</returns>
		public override int LexerToTextControl(ITextControl textControl, int offset) {
			return offset;
		}

		public override bool QuickCheckAvailability(ITextControl textControl, IPsiSourceFile projectFile) {
			return projectFile.LanguageType.Is<T4ProjectFileType>();
		}

		public T4CSharpTypingAssist(Lifetime lifetime, ISolution solution, ICommandProcessor commandProcessor, CachingLexerService cachingLexerService,
			ISettingsStore settingsStore, ITypingAssistManager typingAssistManager, IPsiServices psiServices)
			: base(lifetime, solution, commandProcessor, cachingLexerService, settingsStore, typingAssistManager, psiServices) {
		}

	}

}