using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents an expression block (&lt;#= ... #&gt;).
	/// </summary>
	public class T4ExpressionBlock : T4CodeBlock {

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public override NodeType NodeType {
			get { return T4ElementTypes.T4ExpressionBlock; }
		}

		/// <summary>
		/// Gets the type of starting token.
		/// </summary>
		protected override TokenNodeType StartTokenNodeType {
			get { return T4TokenNodeTypes.ExpressionStart; }
		}

	}

}