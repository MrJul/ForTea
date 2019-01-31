using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ReSharper.ForTea.Parsing {
	
	/// <summary>Factory creating <see cref="T4Lexer"/>.</summary>
	internal sealed class T4LexerFactory : ILexerFactory {

		/// <summary>Gets an unique instance of <see cref="T4LexerFactory"/>.</summary>
		[NotNull]
		public static T4LexerFactory Instance { get; } = new T4LexerFactory();

		/// <summary>Creates a new lexer for a specified buffer.</summary>
		/// <param name="buffer">The buffer onto which the lexer operates.</param>
		/// <returns>A new lexer.</returns>
		public ILexer CreateLexer(IBuffer buffer)
			=> new T4Lexer(buffer);

		private T4LexerFactory() {
		}

	}

}