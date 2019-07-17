using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	public class T4AttributeValue : T4CompositeElement, IT4AttributeValue
	{
		public override NodeType NodeType => T4ElementTypes.T4AttributeValue;
		protected override T4TokenRole GetChildRole(NodeType nodeType) => T4TokenRole.Value;
	}
}
