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
using GammaJul.ReSharper.ForTea.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Base, file independent language service for T4.
	/// </summary>
	[Language(typeof(T4Language))]
	public sealed class T4LanguageService : LanguageService {
		
		private readonly T4Environment _t4Environment;
		private readonly DirectiveInfoManager _directiveInfoManager;

		/// <summary>
		/// Creates a lexer that filters tokens that have no meaning.
		/// </summary>
		/// <param name="lexer">The base lexer.</param>
		/// <returns>An implementation of a filtering lexer.</returns>
		public override ILexer CreateFilteringLexer(ILexer lexer) {
			return new T4FilteringLexer(lexer);
		}

		/// <summary>
		/// Gets a factory capable of creating T4 lexers.
		/// </summary>
		/// <returns>An implementation of <see cref="ILexerFactory"/>.</returns>
		public override ILexerFactory GetPrimaryLexerFactory() {
			return T4LexerFactory.Instance;
		}
		
		/// <summary>
		/// Creates a parser for a given PSI source file.
		/// </summary>
		/// <param name="lexer">The lexer that the parser will use.</param>
		/// <param name="module">The module owning the source file.</param>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>A T4 parser that operates onto <paramref name="lexer"/>.</returns>
		public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile) {
			return new T4Parser(_t4Environment, _directiveInfoManager, lexer, sourceFile);
		}

		/// <summary>
		/// Gets a cache provider for T4 files.
		/// TODO: implement a cache provider
		/// </summary>
		public override ILanguageCacheProvider CacheProvider {
			get { return null; }
		}

		public override bool SupportTypeMemberCache {
			get { return false; }
		}

		/// <summary>
		/// Gets a type presenter.
		/// </summary>
		public override ITypePresenter TypePresenter {
			get { return DefaultTypePresenter.Instance; }
		}

		public override bool IsCaseSensitive {
			get { return true; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4LanguageService"/> class.
		/// </summary>
		/// <param name="t4Language">The T4 language.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		/// <param name="constantValueService">The constant value service.</param>
		/// <param name="t4Environment">An object describing the environment for T4 files.</param>
		public T4LanguageService([NotNull] T4Language t4Language, [NotNull] DirectiveInfoManager directiveInfoManager,
			[NotNull] IConstantValueService constantValueService, [NotNull] T4Environment t4Environment)
			: base(t4Language, constantValueService) {
			_t4Environment = t4Environment;
			_directiveInfoManager = directiveInfoManager;
		}

	}

}