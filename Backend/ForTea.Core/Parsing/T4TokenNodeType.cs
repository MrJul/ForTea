using System;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ForTea.Core.Parsing {
	
	/// <summary>Represents a T4 token node type.</summary>
	public sealed class T4TokenNodeType : TokenNodeType {

		private readonly T4TokenNodeFlag _flag;

		/// <summary>Creates a new token having this token type.</summary>
		/// <param name="buffer">The buffer holding the token text.</param>
		/// <param name="startOffset">The token starting offset in <paramref name="buffer"/>.</param>
		/// <param name="endOffset">The token ending offset in <paramref name="buffer"/>.</param>
		/// <returns>A new instance of <see cref="T4Token"/>.</returns>
		public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
			=> new T4Token(this, buffer, startOffset, endOffset);

		/// <summary>Gets the fixed string representation of the token type if available, <see cref="String.Empty"/> otherwise.</summary>
		public override string TokenRepresentation { get; }

		/// <summary>Gets whether the token type is a block tag.</summary>
		public bool IsTag
			=> _flag == T4TokenNodeFlag.Tag;

		/// <summary>Gets whether the token type is a whitespace.</summary>
		public override bool IsWhitespace
			=> _flag == T4TokenNodeFlag.Whitespace;

		/// <summary>Gets whether the token type is a comment.</summary>
		/// <remarks>Always returns <c>false</c>.</remarks>
		public override bool IsComment
			=> false;

		/// <summary>Gets whether the token type is a string literal.</summary>
		public override bool IsStringLiteral
			=> _flag == T4TokenNodeFlag.StringLiteral;

		/// <summary>Gets whether the token type is a constant literal.</summary>
		/// <remarks>Always returns <c>false</c>.</remarks>
		public override bool IsConstantLiteral
			=> false;

		/// <summary>Gets whether the token type is an identifier.</summary>
		public override bool IsIdentifier
			=> _flag == T4TokenNodeFlag.Identifier;

		/// <summary>Gets whether the token type is a keyword.</summary>
		/// <remarks>Always returns <c>false</c>.</remarks>
		public override bool IsKeyword
			=> false;

		public override bool IsFiltered
			=> _flag == T4TokenNodeFlag.Whitespace;

		/// <summary>Initializes a new instance of the <see cref="T4TokenNodeType"/> class.</summary>
		/// <param name="name">The token type name.</param>
		/// <param name="index">An unique index for this token node type.</param>
		/// <param name="repr">The static token type representation.</param>
		/// <param name="flag">The special type of token.</param>
		internal T4TokenNodeType([NotNull] string name, int index, [CanBeNull] string repr, T4TokenNodeFlag flag)
			: base(name, index) {
			_flag = flag;
			TokenRepresentation = repr ?? String.Empty;
		}

	}

}
