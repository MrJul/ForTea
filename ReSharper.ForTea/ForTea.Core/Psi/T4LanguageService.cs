using System.Collections.Generic;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {

	/// <summary>Base, file independent language service for T4.</summary>
	[Language(typeof(T4Language))]
	public sealed class T4LanguageService : LanguageService {
		
		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;

		/// <summary>Creates a lexer that filters tokens that have no meaning.</summary>
		/// <param name="lexer">The base lexer.</param>
		/// <returns>An implementation of a filtering lexer.</returns>
		public override ILexer CreateFilteringLexer(ILexer lexer)
			=> new T4FilteringLexer(lexer);

		/// <summary>Gets a factory capable of creating T4 lexers.</summary>
		/// <returns>An implementation of <see cref="ILexerFactory"/>.</returns>
		public override ILexerFactory GetPrimaryLexerFactory()
			=> T4LexerFactory.Instance;

		/// <summary>Creates a parser for a given PSI source file.</summary>
		/// <param name="lexer">The lexer that the parser will use.</param>
		/// <param name="module">The module owning the source file.</param>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>A T4 parser that operates onto <paramref name="lexer"/>.</returns>
		public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile)
			=> new T4Parser(_t4Environment, _directiveInfoManager, lexer, sourceFile);

		/// <summary>
		/// Gets a cache provider for T4 files.
		/// TODO: implement a cache provider
		/// </summary>
		public override ILanguageCacheProvider CacheProvider
			=> null;

		public override bool SupportTypeMemberCache
			=> false;

		/// <summary>Gets a type presenter.</summary>
		public override ITypePresenter TypePresenter
			=> DefaultTypePresenter.Instance;

		public override bool IsCaseSensitive
			=> true;

		public override IEnumerable<ITypeDeclaration> FindTypeDeclarations(IFile file)
			=> EmptyList<ITypeDeclaration>.InstanceList;

		/// <summary>Initializes a new instance of the <see cref="T4LanguageService"/> class.</summary>
		/// <param name="t4Language">The T4 language.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="T4DirectiveInfoManager"/>.</param>
		/// <param name="constantValueService">The constant value service.</param>
		/// <param name="t4Environment">An object describing the environment for T4 files.</param>
		public T4LanguageService(
			[NotNull] T4Language t4Language,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] IConstantValueService constantValueService,
			[NotNull] IT4Environment t4Environment
		)
			: base(t4Language, constantValueService) {
			_t4Environment = t4Environment;
			_directiveInfoManager = directiveInfoManager;
		}

	}

}