using System;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	[SolutionComponent]
	public class AutopopupInDirective : IAutomaticCodeCompletionStrategy {

		[NotNull] private readonly SettingsScalarEntry _settingsEntry;

		public bool AcceptTyping(char c, ITextControl textControl, IContextBoundSettingsStore boundSettingsStore)
			=> Char.IsLetterOrDigit(c) || c == ' ' || c == '"';

		public bool ProcessSubsequentTyping(char c, ITextControl textControl)
			=> Char.IsLetterOrDigit(c);

		public bool AcceptsFile(IFile file, ITextControl textControl)
			=> file is IT4File && this.MatchTokenType(file, textControl, IsSupportedTokenType);

		private static bool IsSupportedTokenType(TokenNodeType tokenType)
			=> tokenType == T4TokenNodeTypes.Name
			|| tokenType == T4TokenNodeTypes.Space
			|| tokenType == T4TokenNodeTypes.DirectiveStart
			|| tokenType == T4TokenNodeTypes.Quote
			|| tokenType == T4TokenNodeTypes.Value;

		public AutopopupType IsEnabledInSettings(IContextBoundSettingsStore settingsStore, ITextControl textControl)
			=> (AutopopupType) settingsStore.GetValue(_settingsEntry, null);

		public PsiLanguageType Language
			=> T4Language.Instance;

		public bool ForceHideCompletion
			=> false;

		public AutopopupInDirective([NotNull] ISettingsStore settingsStore) {
			_settingsEntry = settingsStore.Schema.GetScalarEntry<T4AutopopupSettingsKey, AutopopupType>(key => key.InDirectives);
		}

	}

}