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
using System;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	[SolutionComponent]
	public class AutopopupInDirective : IAutomaticCodeCompletionStrategy {

		[NotNull] private readonly SettingsScalarEntry _settingsEntry;

		public bool AcceptTyping(char c, ITextControl textControl, IContextBoundSettingsStore boundSettingsStore) {
			return Char.IsLetterOrDigit(c) || c == ' ' || c == '"';
		}

		public bool ProcessSubsequentTyping(char c, ITextControl textControl) {
			return Char.IsLetterOrDigit(c);
		}

		public bool AcceptsFile(IFile file, ITextControl textControl) {
			return file is IT4File && this.MatchTokenType(file, textControl, IsSupportedTokenType);
		}

		private static bool IsSupportedTokenType(TokenNodeType tokenType) {
			return tokenType == T4TokenNodeTypes.Name
				|| tokenType == T4TokenNodeTypes.Space
				|| tokenType == T4TokenNodeTypes.DirectiveStart
				|| tokenType == T4TokenNodeTypes.Quote
				|| tokenType == T4TokenNodeTypes.Value;
		}

		public AutopopupType IsEnabledInSettings(IContextBoundSettingsStore settingsStore, ITextControl textControl) {
			return (AutopopupType) settingsStore.GetValue(_settingsEntry, null);
		}

		public PsiLanguageType Language {
			get { return T4Language.Instance; }
		}
		
		public bool ForceHideCompletion {
			get { return false; }
		}

		public AutopopupInDirective([NotNull] ISettingsStore settingsStore) {
			_settingsEntry = settingsStore.Schema.GetScalarEntry<T4AutopopupSettingsKey, AutopopupType>(key => key.InDirectives);
		}

	}

}