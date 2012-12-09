using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Base, file independent language service for T4.
	/// </summary>
	[Language(typeof(T4Language))]
	public sealed class T4LanguageService : LanguageService {
		private static readonly T4WordIndexProvider _indexProvider = new T4WordIndexProvider();
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
		/// Gets a word index language provider for T4.
		/// </summary>
		/// <returns>An implementation of <see cref="IWordIndexLanguageProvider"/>.</returns>
		public override IWordIndexLanguageProvider WordIndexLanguageProvider {
			get { return _indexProvider; }
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
		/// Determines whether a given node is filtered.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <returns><c>true</c> if <paramref name="node"/> is a whitespace; otherwise, <c>false</c>.</returns>
		public override bool IsFilteredNode(ITreeNode node) {
			TokenNodeType nodeType = node.GetTokenType();
			return nodeType != null && nodeType.IsWhitespace;
		}

		/// <summary>
		/// Gets a cache provider for T4 files.
		/// TODO: implement a cache provider
		/// </summary>
		public override ILanguageCacheProvider CacheProvider {
			get { return null; }
		}

		public override bool SupportTypeMemberCache {
			get { return true; }
		}

		/// <summary>
		/// Gets a type presenter.
		/// </summary>
		public override ITypePresenter TypePresenter {
			get { return DefaultTypePresenter.Instance; }
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