using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ReSharper.ForTea.Parsing {

	/// <summary>
	/// T4 lexer implementation.
	/// </summary>
	internal sealed class T4Lexer : IIncrementalLexer {

		private readonly IBuffer _buffer;
		private int _pos;
		private int _length;
		private int _tokenStart;
		private int _tokenEnd;
		private ScanMode _scanMode;
		private T4TokenNodeType _currentTokenType;

		private sealed class State {
			internal int Pos;
			internal int TokenStart;
			internal int TokenEnd;
			internal ScanMode ScanMode;
			internal T4TokenNodeType CurrentTokenType;
		}

		private enum ScanMode : uint {
			Text,
			Code,
			Directive,
			AttributeValue
		}


		/// <summary>
		/// Scans for the next token and updates token start and end positions.
		/// </summary>
		/// <returns>The type of the next token, or <c>null</c> if the end was reached.</returns>
		[CanBeNull]
		private T4TokenNodeType Scan() {
			_tokenStart = _pos;
			T4TokenNodeType tokenType = ScanCore();
			_tokenEnd = _pos;
			return tokenType;
		}

		/// <summary>
		/// Scans for the next token.
		/// </summary>
		/// <returns>The type of the next token, or <c>null</c> if the end was reached.</returns>
		[CanBeNull]
		private T4TokenNodeType ScanCore() {
			if (_pos >= _length)
				return null;

			// first test for a tag start or tag end, since they have precedence over everything in a T4 template
			if (_pos + 1 < _length) {
				switch (_buffer[_pos]) {

					// <#@, <#+, <#=, <#
					case '<':
						if ( _buffer[_pos + 1] == '#') {
							_pos += 2;
							if (_pos < _length) {
								switch (_buffer[_pos]) {
									case '@':
										++_pos;
										_scanMode = ScanMode.Directive;
										return T4TokenNodeTypes.DirectiveStart;
									case '+':
										++_pos;
										_scanMode = ScanMode.Code;
										return T4TokenNodeTypes.FeatureStart;
									case '=':
										++_pos;
										_scanMode = ScanMode.Code;
										return T4TokenNodeTypes.ExpressionStart;
								}
							}
							_scanMode = ScanMode.Code;
							return T4TokenNodeTypes.StatementStart;
						}
						break;

					// #>
					case '#':
						if (_buffer[_pos + 1] == '>') {
							_pos += 2;
							_scanMode = ScanMode.Text;
							return T4TokenNodeTypes.BlockEnd;
						}
						break;

				}
			}

			// we have text, code or directive
			switch (_scanMode) {
				case ScanMode.Text:
					return ScanText();
				case ScanMode.Code:
					return ScanCode();
				case ScanMode.Directive:
					return ScanDirective();
				case ScanMode.AttributeValue:
					return ScanAttributeValue();
				default:
					throw new InvalidOperationException("Unknown state " + _scanMode);
			}
		}

		/// <summary>
		/// Determines whether the current character is the start of another T4 tag.
		/// </summary>
		/// <returns><c>true</c> if the current position is a new T4 tag; otherwise, <c>false</c>.</returns>
		private bool IsCurrentCharTag() {
			return _pos + 1 < _length
				&& ((_buffer[_pos] == '<' && _buffer[_pos + 1] == '#')
			        || (_buffer[_pos] == '#' && _buffer[_pos + 1] == '>'));
		}

		/// <summary>
		/// Scans text (part of the file that is not directive nor code).
		/// </summary>
		/// <returns>The type of token found: <see cref="T4TokenNodeTypes.Text"/> or <see cref="T4TokenNodeTypes.NewLine"/>.</returns>
		[NotNull]
		private T4TokenNodeType ScanText() {
			char c = _buffer[_pos];
			
			if (c == '\r') {
				++_pos;
				if (_pos < _length && _buffer[_pos] == '\n')
					++_pos;
				return T4TokenNodeTypes.NewLine;
			}

			if (c == '\n') {
				++_pos;
				return T4TokenNodeTypes.NewLine;
			}

			++_pos;
			while (_pos < _length) {
				c = _buffer[_pos];
				if (IsCurrentCharTag() || c == '\r' || c == '\n')
					break;
				++_pos;
			}
			return T4TokenNodeTypes.Text;
		}

		/// <summary>
		/// Scans a code block.
		/// </summary>
		/// <returns>Always <see cref="T4TokenNodeTypes.Code"/>.</returns>
		[NotNull]
		private T4TokenNodeType ScanCode() {
			do {
				if (IsCurrentCharTag())
					break;
				++_pos;
			}
			while (_pos < _length);
			return T4TokenNodeTypes.Code;
		}

		/// <summary>
		/// Scans a directive.
		/// </summary>
		/// <returns>
		/// One of <see cref="T4TokenNodeTypes.Quote"/>, <see cref="T4TokenNodeTypes.Equal"/>,
		/// <see cref="T4TokenNodeTypes.Space"/> or <see cref="T4TokenNodeTypes.Name"/>.
		/// </returns>
		[NotNull]
		private T4TokenNodeType ScanDirective() {
			char c = _buffer[_pos];

			if (c == '"') {
				++_pos;
				_scanMode = ScanMode.AttributeValue;
				return T4TokenNodeTypes.Quote;
			}
			
			if (c == '=') {
				++_pos;
				return T4TokenNodeTypes.Equal;
			}

			if (c == '\r') {
				++_pos;
				if (_pos < _length && _buffer[_pos] == '\n')
					++_pos;
				return T4TokenNodeTypes.NewLine;
			}

			if (c == '\n') {
				++_pos;
				return T4TokenNodeTypes.NewLine;
			}

			bool isWhiteSpace = Char.IsWhiteSpace(c);
			++_pos;

			while (_pos < _length) {
				c = _buffer[_pos];
				if (IsCurrentCharTag() || c == '"' || c == '=' || c == '\r' || c == '\n' || Char.IsWhiteSpace(c) != isWhiteSpace)
					break;
				++_pos;
			}
			return isWhiteSpace ? T4TokenNodeTypes.Space : T4TokenNodeTypes.Name;
		}

		/// <summary>
		/// Scans an attribute value.
		/// </summary>
		/// <returns>One of <see cref="T4TokenNodeTypes.Quote"/> or <see cref="T4TokenNodeTypes.Value"/>.</returns>
		[NotNull]
		private T4TokenNodeType ScanAttributeValue() {

			if (_buffer[_pos] == '"') {
				++_pos;
				_scanMode = ScanMode.Directive;
				return T4TokenNodeTypes.Quote;
			}

			++_pos;
			while (_pos < _length && !IsCurrentCharTag()) {
				switch (_buffer[_pos]) {
					case '"':
						return T4TokenNodeTypes.Value;
					case '\\':
						if (_pos + 1 < _length && _buffer[_pos + 1] == '"')
							++_pos;
						break;
				}
				++_pos;
			}
			return T4TokenNodeTypes.Value;
		}


		private void LocateToken() {
			if (_currentTokenType == null)
				_currentTokenType = Scan();
		}

		public object CurrentPosition {
			get { return SaveState(); }
			set { RestoreState(value as State); }
		}

		/// <summary>
		/// Starts lexing the whole buffer.
		/// </summary>
		public void Start() {
			_pos = 0;
			_length = _buffer.Length;
			_scanMode = ScanMode.Text;
			_currentTokenType = null;
		}

		/// <summary>
		/// Starts lexing a part of the buffer.
		/// </summary>
		/// <param name="startOffset">The starting offset.</param>
		/// <param name="endOffset">The ending offset.</param>
		/// <param name="state">The scan mode of the lexer.</param>
		public void Start(int startOffset, int endOffset, uint state) {
			_pos = startOffset;
			_length = endOffset;
			_scanMode = (ScanMode) state;
			_currentTokenType = null;
		}

		/// <summary>
		/// Advances the lexer to the next token.
		/// </summary>
		public void Advance() {
			LocateToken();
			_currentTokenType = null;
		}

		/// <summary>
		/// Saves the lexer state.
		/// </summary>
		/// <returns>An instance of <see cref="State"/>.</returns>
		[NotNull]
		private State SaveState() {
			return new State {
				Pos = _pos,
				TokenStart = _tokenStart,
				TokenEnd = _tokenEnd,
				ScanMode = _scanMode,
				CurrentTokenType = _currentTokenType
			};
		}

		/// <summary>
		/// Restores the lexer state.
		/// </summary>
		/// <param name="state">An instance of <see cref="State"/>.</param>
		private void RestoreState([CanBeNull] State state) {
			if (state == null)
				return;

			_pos = state.Pos;
			_tokenStart = state.TokenStart;
			_tokenEnd = state.TokenEnd;
			_scanMode = state.ScanMode;
			_currentTokenType = state.CurrentTokenType;
		}

		/// <summary>
		/// Gets the current token node type.
		/// </summary>
		public TokenNodeType TokenType {
			get {
				LocateToken();
				return _currentTokenType;
			}
		}

		/// <summary>
		/// Gets the current token starting position.
		/// </summary>
		public int TokenStart {
			get {
				LocateToken();
				return _tokenStart;
			}
		}

		/// <summary>
		/// Gets the current token ending position.
		/// </summary>
		public int TokenEnd {
			get {
				LocateToken();
				return _tokenEnd;
			}
		}

		/// <summary>
		/// Gets the buffer onto which this lexer operates.
		/// </summary>
		public IBuffer Buffer {
			get { return _buffer; }
		}
		
		/// <summary>
		/// Number of lexems that incremental re-lexing should step back to start relexing.
		/// </summary>
		public int LexemIndent {
			get { return 7; }
		}

		/// <summary>
		/// Gets the end of buffer position.
		/// </summary>
		public int EOFPos {
			get { return _length; }
		}

		public uint LexerStateEx {
			get {
				LocateToken();
				return (uint) _scanMode;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4Lexer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer onto which this lexer operates.</param>
		internal T4Lexer([NotNull] IBuffer buffer) {
			if (buffer == null)
				throw new ArgumentNullException("buffer");

			_buffer = buffer;
		}

	}

}