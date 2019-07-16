using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ForTea.Core.Parsing.Lexing
{
	internal partial class T4LexerGenerated : IIncrementalLexer
	{
		private readonly struct T4LexerState
		{
			public TokenNodeType CurrentTokenType { get; }
			public int YyBufferIndex { get; }
			public int YyBufferStart { get; }
			public int YyBufferEnd { get; }
			public int YyLexicalState { get; }

			public T4LexerState(
				TokenNodeType currentTokenType,
				int yyBufferIndex,
				int yyBufferStart,
				int yyBufferEnd,
				int yyLexicalState
			)
			{
				CurrentTokenType = currentTokenType;
				YyBufferIndex = yyBufferIndex;
				YyBufferStart = yyBufferStart;
				YyBufferEnd = yyBufferEnd;
				YyLexicalState = yyLexicalState;
			}
		}

		private TokenNodeType myCurrentTokenType;

		public void Start() => Start(0, yy_buffer.Length, YYINITIAL);

		public void Start(int startOffset, int endOffset, uint state)
		{
			yy_buffer_index = startOffset;
			yy_buffer_start = startOffset;
			yy_buffer_end = startOffset;
			yy_eof_pos = endOffset;
			yy_lexical_state = (int) state;
			myCurrentTokenType = null;
		}

		public void Advance()
		{
			myCurrentTokenType = null;
			LocateToken();
		}

		public object CurrentPosition
		{
			get => new T4LexerState(
				myCurrentTokenType,
				yy_buffer_index,
				yy_buffer_start,
				yy_buffer_end,
				yy_lexical_state
			);
			set
			{
				var state = (T4LexerState) value;
				myCurrentTokenType = state.CurrentTokenType;
				yy_buffer_index = state.YyBufferIndex;
				yy_buffer_start = state.YyBufferStart;
				yy_buffer_end = state.YyBufferEnd;
				yy_lexical_state = state.YyLexicalState;
			}
		}

		public TokenNodeType TokenType
		{
			get
			{
				LocateToken();
				return myCurrentTokenType;
			}
		}

		public int TokenStart
		{
			get
			{
				LocateToken();
				return yy_buffer_start;
			}
		}

		public int TokenEnd
		{
			get
			{
				LocateToken();
				return yy_buffer_end;
			}
		}

		public IBuffer Buffer => yy_buffer;
		public uint LexerStateEx => (uint) yy_lexical_state;
		public int LexemIndent => 7;
		public int EOFPos => yy_eof_pos;
		private TokenNodeType makeToken(TokenNodeType type) => myCurrentTokenType = type;

		private void LocateToken()
		{
			if (myCurrentTokenType == null)
			{
				myCurrentTokenType = advance();
			}
		}
	}
}
