using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Base class for all T4 composite elements.
	/// </summary>
	public abstract class T4CompositeElement : CompositeElement {

		public override PsiLanguageType Language {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the child role of a child element.
		/// </summary>
		/// <param name="child">The child element.</param>
		/// <returns></returns>
		public sealed override short GetChildRole(TreeElement child) {
			return (short) GetChildRole(child.NodeType);
		}

		/// <summary>
		/// Gets the role of a child node.
		/// </summary>
		/// <param name="nodeType">The type of the child node</param>
		protected abstract T4TokenRole GetChildRole([NotNull] NodeType nodeType);

	}

}