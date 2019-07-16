using GammaJul.ForTea.Core.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ForTea.Core.Tree.Impl {

	/// <summary>Implementation of <see cref="IT4CodeBlock"/>.</summary>
	public abstract class T4CodeBlock : T4CompositeElement, IT4CodeBlock {

		/// <summary>Gets the role of a child node.</summary>
		/// <param name="nodeType">The type of the child node</param>
		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			if (nodeType == T4TokenNodeTypes.RAW_CODE)
				return T4TokenRole.Code;
			if (nodeType == T4TokenNodeTypes.BLOCK_END)
				return T4TokenRole.BlockEnd;
			if (nodeType == StartTokenNodeType)
				return T4TokenRole.BlockStart;
			return T4TokenRole.Unknown;
		}

		/// <summary>Gets the type of starting token.</summary>
		protected abstract TokenNodeType StartTokenNodeType { get; }

		/// <summary>Gets the text of the code block.</summary>
		/// <returns>The code text, or <c>null</c> if none is available.</returns>
		public string GetCodeText()
			=> GetCodeToken()?.GetText();

		/// <summary>Gets the start token of the block.</summary>
		/// <returns>A block start token.</returns>
		public IT4Token GetStartToken()
			=> (IT4Token) this.GetChildByRole((short) T4TokenRole.BlockStart);

		/// <summary>Gets the end token of the block.</summary>
		/// <returns>A block end token, or <c>null</c> if none is available.</returns>
		public IT4Token GetEndToken()
			=> (IT4Token) FindChildByRole((short) T4TokenRole.BlockEnd);

		/// <summary>Gets the code token.</summary>
		/// <returns>The code token, or <c>null</c> if none is available.</returns>
		public IT4Token GetCodeToken()
			=> (IT4Token) FindChildByRole((short) T4TokenRole.Code);

	}

}