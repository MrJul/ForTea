using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4FileDataCache {

		private void RegisterPsiChanged([NotNull] Lifetime lifetime, [NotNull] PsiManager psiFiles) {
			lifetime.AddBracket(
				() => psiFiles.PhysicalPsiChanged += OnPsiChanged,
				() => psiFiles.PhysicalPsiChanged -= OnPsiChanged);
		}

		/// <summary>
		/// Called when a PSI element changes.
		/// </summary>
		/// <param name="treeNode">The tree node that changed.</param>
		/// <param name="psiChangedElementType">The type of the PSI change.</param>
		private void OnPsiChanged(ITreeNode treeNode, PsiChangedElementType psiChangedElementType) {
			if (treeNode != null && psiChangedElementType == PsiChangedElementType.CONTENTS_CHANGED)
				OnPsiFileChanged(treeNode.GetContainingFile());
		}

	}

}
