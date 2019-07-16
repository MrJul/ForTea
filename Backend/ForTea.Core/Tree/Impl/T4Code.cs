using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	public class T4Code : T4CompositeElement, IT4Code
	{
		public override NodeType NodeType => T4ElementTypes.T4Code;
		protected override T4TokenRole GetChildRole(NodeType nodeType) => T4TokenRole.Code;
	}
}
