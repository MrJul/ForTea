using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a directive, like <c>&lt;#@ import namespace="System"#&gt;</c>
	/// </summary>
	public class T4Directive : T4CompositeElement, IT4Directive {

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public override NodeType NodeType {
			get { return T4ElementTypes.T4Directive; }
		}

		/// <summary>
		/// Gets the role of a child node.
		/// </summary>
		/// <param name="nodeType">The type of the child node</param>
		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			if (nodeType == T4ElementTypes.T4DirectiveAttribute)
				return T4TokenRole.Attribute;
			if (nodeType == T4TokenNodeTypes.BlockEnd)
				return T4TokenRole.BlockEnd;
			if (nodeType == T4TokenNodeTypes.DirectiveStart)
				return T4TokenRole.BlockStart;
			if (nodeType == T4TokenNodeTypes.Name)
				return T4TokenRole.Name;
			return T4TokenRole.Unknown;
		}

		/// <summary>
		/// Gets the start token of the block.
		/// </summary>
		/// <returns>A block start token.</returns>
		public IT4Token GetStartToken() {
			return (IT4Token) this.GetChildByRole((short) T4TokenRole.BlockStart);
		}

		/// <summary>
		/// Gets the token representing the name of this node.
		/// </summary>
		/// <returns>The name token, or <c>null</c> if none is available.</returns>
		public IT4Token GetNameToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.Name);
		}

		/// <summary>
		/// Gets the end token of the block.
		/// </summary>
		/// <returns>A block end token, or <c>null</c> if none is available.</returns>
		public IT4Token GetEndToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.BlockEnd);
		}

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		/// <returns>The node name, or <c>null</c> if none is available.</returns>
		public string GetName() {
			IT4Token nameToken = GetNameToken();
			return nameToken != null ? nameToken.GetText() : null;
		}

		/// <summary>
		/// Returns the attributes of the directive.
		/// </summary>
		public IEnumerable<IT4DirectiveAttribute> GetAttributes() {
			return FindChildrenByRole<IT4DirectiveAttribute>((short) T4TokenRole.Attribute);
		}

		/// <summary>
		/// Returns an attribute that has a given name.
		/// </summary>
		/// <param name="name">The name of the attribute to retrieve.</param>
		/// <returns>An instance of <see cref="IT4DirectiveAttribute"/>, or <c>null</c> if none had the name <paramref name="name"/>.</returns>
		public IT4DirectiveAttribute GetAttribute(string name) {
			return String.IsNullOrEmpty(name)
				? null
				: GetAttributes().FirstOrDefault(att => name.Equals(att.GetName(), StringComparison.OrdinalIgnoreCase));
		}

		public override string ToString() {
			return GetText();
		}

	}

}