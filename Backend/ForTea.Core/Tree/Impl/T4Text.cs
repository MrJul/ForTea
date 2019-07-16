using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	public class T4Text : T4CompositeElement, IT4Text
	{
		public override NodeType NodeType => T4ElementTypes.T4Text;
		protected override T4TokenRole GetChildRole(NodeType nodeType) => T4TokenRole.Unknown;
	}
}
