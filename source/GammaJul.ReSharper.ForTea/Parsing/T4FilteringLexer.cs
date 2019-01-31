using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Parsing {

	/// <summary>Lexer filtering whitespaces.</summary>
	internal sealed class T4FilteringLexer : FilteringLexer {

		protected override bool Skip(TokenNodeType tokenType)
			=> tokenType.IsWhitespace;

		/// <summary>Initializes a new instance of the <see cref="T4FilteringLexer"/> class.</summary>
		/// <param name="lexer">The base lexer.</param>
		public T4FilteringLexer([NotNull] ILexer lexer)
			: base(lexer) {
		}

	}

}