using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application.UI.Controls.TreeView;
using JetBrains.Application.UI.Controls.Utils;
using JetBrains.Application.UI.TreeModels;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Pointers;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Services.CodeStructure {

	internal abstract class T4CodeStructureElement<T> : CodeStructureElement, IFileStructureAspect, IMemberNavigationAspect
	where T : class, ITreeNode {

		private readonly TextRange _textRange;
		[NotNull] private readonly ITreeNodePointer<T> _pointer;
		
		[CanBeNull]
		protected T GetTreeNode()
			=> _pointer.GetTreeNode();

		public override TextRange GetTextRange()
			=> _textRange;

		public override ITreeNode TreeNode
			=> GetTreeNode();

		public override PsiLanguageType Language
			=> T4Language.Instance;

		public override IFileStructureAspect GetFileStructureAspect()
			=> this;

		public override IGotoFileMemberAspect GetGotoMemberAspect()
			=> null;

		public override IMemberNavigationAspect GetMemberNavigationAspect()
			=> this;

		public abstract void Present(
			StructuredPresenter<TreeModelNode, IPresentableItem> presenter,
			IPresentableItem item,
			TreeModelNode modelNode,
			PresentationState state
		);

		public abstract IList<string> GetQuickSearchTexts();

		bool IFileStructureAspect.CanMoveElements(RelativeLocation location, IList<CodeStructureElement> dropElements)
			=> false;

		void IFileStructureAspect.MoveElements(RelativeLocation location, IList<CodeStructureElement> dropElements)
			=> throw new NotSupportedException();

		bool IFileStructureAspect.CanRemove()
			=> false;

		void IFileStructureAspect.Remove()
			=> throw new NotSupportedException();

		bool IFileStructureAspect.CanRename()
			=> false;

		string IFileStructureAspect.InitialName()
			=> throw new NotSupportedException();

		void IFileStructureAspect.Rename(string newName)
			=> throw new NotSupportedException();

		public virtual DocumentRange NavigationRange
			=> GetTreeNode()?.GetNavigationRange() ?? DocumentRange.InvalidRange;

		bool IFileStructureAspect.InitiallyExpanded
			=> true;

		DocumentRange[] IMemberNavigationAspect.GetNavigationRanges() {
			DocumentRange navigationRange = NavigationRange;
			return navigationRange.IsValid() ? new[] { navigationRange } : EmptyArray<DocumentRange>.Instance;
		}

		protected T4CodeStructureElement([NotNull] CodeStructureElement parent, [NotNull] T treeNode)
			: base(parent) {
			_textRange = treeNode.GetDocumentRange().TextRange;
			_pointer = treeNode.CreateTreeElementPointer();
		}

	}

}