using GammaJul.ForTea.Core.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ForTea.Core.Tree.Impl {

	/// <summary>Represents an expression block (&lt;#= ... #&gt;).</summary>
	public class T4ExpressionBlock : T4CodeBlock {

		/// <summary>Gets the node type of this element.</summary>
		public override NodeType NodeType
			=> T4ElementTypes.T4ExpressionBlock;

		/// <summary>Gets the type of starting token.</summary>
		protected override TokenNodeType StartTokenNodeType
			=> T4TokenNodeTypes.EXPRESSION_BLOCK_START;

	}

}