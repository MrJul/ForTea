using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Parsing {


	/// <summary>
	/// Lexer filtering whitespaces.
	/// </summary>
	internal sealed class T4FilteringLexer : FilteringLexer {

		/// <summary>
		/// Returns whether to skips a specified token type.
		/// </summary>
		/// <param name="tokenType">Type of the token.</param>
		/// <returns><c>true</c> if the token must be skipped, <c>false</c> otherwise.</returns>
		protected override bool Skip(TokenNodeType tokenType) {
			return tokenType.IsWhitespace;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4FilteringLexer"/> class.
		/// </summary>
		/// <param name="lexer">The base lexer.</param>
		internal T4FilteringLexer(ILexer lexer)
			: base(lexer) {
		}

	}

}