using GammaJul.ForTea.Core.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree {

	/// <summary>Represents a directive attribute, like <c>namespace="System"</c> in an import directive.</summary>
	public sealed class T4DirectiveAttribute : T4CompositeElement, IT4DirectiveAttribute {

		/// <summary>Gets the role of a child node.</summary>
		/// <param name="nodeType">The type of the child node</param>
		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			if (nodeType == T4TokenNodeTypes.Name)
				return T4TokenRole.Name;
			if (nodeType == T4TokenNodeTypes.Equal)
				return T4TokenRole.Separator;
			if (nodeType == T4TokenNodeTypes.Value)
				return T4TokenRole.Value;
			return T4TokenRole.Unknown;
		}

		/// <summary>Gets the node type of this element.</summary>
		public override NodeType NodeType
			=> T4ElementTypes.T4DirectiveAttribute;

		/// <summary>Gets the token representing the name of this node.</summary>
		/// <returns>The name token, or <c>null</c> if none is available.</returns>
		public IT4Token GetNameToken()
			=> (IT4Token) FindChildByRole((short) T4TokenRole.Name);

		/// <summary>Gets the token representing the equal sign between the name and the value of this attribute.</summary>
		/// <returns>An equal token, or <c>null</c> if none is available.</returns>
		public IT4Token GetEqualToken()
			=> (IT4Token) FindChildByRole((short) T4TokenRole.Separator);

		/// <summary>Gets the token representing the value of this attribute.</summary>
		/// <returns>A value token, or <c>null</c> if none is available.</returns>
		public IT4Token GetValueToken()
			=> (IT4Token) FindChildByRole((short) T4TokenRole.Value);

		/// <summary>Gets the name of the node.</summary>
		/// <returns>The node name, or <c>null</c> if none is available.</returns>
		public string GetName()
			=> GetNameToken()?.GetText();

		/// <summary>Gets the value of this attribute.</summary>
		/// <returns>The attribute value, or <c>null</c> if none is available.</returns>
		public string GetValue()
			=> GetValueToken()?.GetText();

		/// <summary>Gets or sets the error associated with the value that have been identified at parsing time.</summary>
		public string ValueError { get; set; }

	}

}