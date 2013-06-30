using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;

namespace GammaJul.ReSharper.ForTea.Psi.CodeStyle {

	public partial class T4CodeFormatter {


		public override ITreeNode[] CreateSpace(string indent, ITreeNode rightNonSpace, ITreeNode replacedSpace) {
			return new ITreeNode[] {
				TreeElementFactory.CreateLeafElement(T4TokenNodeTypes.Space, new StringBuffer(indent), 0, indent.Length)
			};
		}

	}

}