using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a feature block (&lt;#+ ... #&gt;).
	/// </summary>
	public class T4FeatureBlock : T4CodeBlock {

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public override NodeType NodeType {
			get { return T4ElementTypes.T4FeatureBlock; }
		}

		/// <summary>
		/// Gets the type of starting token.
		/// </summary>
		protected override TokenNodeType StartTokenNodeType {
			get { return T4TokenNodeTypes.FeatureStart; }
		}

	}

}