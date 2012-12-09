using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Services.Selection {

	public class T4NodeSelection : TreeNodeSelection<IT4File> {

		public override ISelectedRange Parent {
			get {
				ITreeNode parentNode = TreeNode.Parent;
				return parentNode == null ? null : new T4NodeSelection(FileNode, parentNode);
			}
		}

		public T4NodeSelection(IT4File fileNode, ITreeNode node)
			: base(fileNode, node) {
		}

	}

}