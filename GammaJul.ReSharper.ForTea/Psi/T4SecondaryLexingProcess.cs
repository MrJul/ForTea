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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Secondary lexing process for T4 files, capable of getting a lexer for the code behind file.
	/// </summary>
	internal sealed class T4SecondaryLexingProcess : ISecondaryLexingProcess {
		private readonly PsiLanguageType _codeBehindLanguage;
		private readonly MixedLexer _mixedLexer;

		/// <summary>
		/// Tries to create a lexer for a code behind file.
		/// </summary>
		/// <param name="baseLexer">The base T4 lexer.</param>
		/// <returns>A C# lexer for the current code block, or <c>null</c> if none could be created.</returns>
		public ILexer TryCreateCodeBehindLexer(ILexer baseLexer) {
			if (baseLexer.TokenType == T4TokenNodeTypes.Code) {
				LanguageService service = _codeBehindLanguage.LanguageService();
				if (service != null) {
					var buffer = new ProjectedBuffer(_mixedLexer.Buffer,
						new TextRange(_mixedLexer.PrimaryLexer.TokenStart, _mixedLexer.PrimaryLexer.AdvanceWhile(T4TokenNodeTypes.Code)));
					ILexer lexer = service.GetPrimaryLexerFactory().CreateLexer(buffer);
					lexer.Start();
					return lexer;
				}
			}
			return null;
		}

		/// <summary>
		/// Determines whether this instance can handle the specified project file type.
		/// </summary>
		/// <param name="projectFileType">Type of the project file.</param>
		/// <returns><c>true</c> if this instance can handle the specified project file type; otherwise, <c>false</c>.</returns>
		public bool CanHandle(ProjectFileType projectFileType) {
			return projectFileType.Is<T4ProjectFileType>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4SecondaryLexingProcess"/> class.
		/// </summary>
		/// <param name="codeBehindLanguage">The code behind language.</param>
		/// <param name="mixedLexer">The mixed lexer.</param>
		internal T4SecondaryLexingProcess([NotNull] PsiLanguageType codeBehindLanguage, [NotNull] MixedLexer mixedLexer) {
			_codeBehindLanguage = codeBehindLanguage;
			_mixedLexer = mixedLexer;
		}
	}

}