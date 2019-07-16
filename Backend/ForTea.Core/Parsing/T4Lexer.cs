using GammaJul.ForTea.Core.Parsing.Lexing;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ForTea.Core.Parsing
{
	// Delegation is used instead of inheritance to allow this class to be public
	public sealed class T4Lexer : IIncrementalLexer
	{
		private T4LexerGenerated Generated { get; }
		public T4Lexer(IBuffer buffer) => Generated = new T4LexerGenerated(buffer);

		public object CurrentPosition
		{
			get => Generated.CurrentPosition;
			set => Generated.CurrentPosition = value;
		}

		public void Start() => Generated.Start();
		public void Advance() => Generated.Advance();
		public TokenNodeType TokenType => Generated.TokenType;
		public int TokenStart => Generated.TokenStart;
		public int TokenEnd => Generated.TokenEnd;
		public IBuffer Buffer => Generated.Buffer;
		public uint LexerStateEx => Generated.LexerStateEx;
		public void Start(int startOffset, int endOffset, uint state) => Generated.Start(startOffset, endOffset, state);
		public int EOFPos => Generated.EOFPos;
		public int LexemIndent => Generated.LexemIndent;
	}
}
