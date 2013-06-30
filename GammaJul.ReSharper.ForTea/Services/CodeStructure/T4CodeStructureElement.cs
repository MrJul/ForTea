#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.CommonControls;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.Util;
#if SDK80
using JetBrains.ReSharper.Psi.Pointers;
#endif

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	internal abstract class T4CodeStructureElement<T> : CodeStructureElement, IFileStructureAspect, IMemberNavigationAspect
		where T : class, ITreeNode {

		private readonly TextRange _textRange;
		private readonly ITreeNodePointer<T> _pointer;
		
		[CanBeNull]
		protected T GetTreeNode() {
			return _pointer.GetTreeNode();
		}

		public override TextRange GetTextRange() {
			return _textRange;
		}

		public override ITreeNode TreeNode {
			get { return GetTreeNode(); }
		}

		public override PsiLanguageType Language {
			get { return T4Language.Instance; }
		}

		public override IFileStructureAspect GetFileStructureAspect() {
			return this;
		}

		public override IGotoFileMemberAspect GetGotoMemberAspect() {
			return null;
		}

		public override IMemberNavigationAspect GetMemberNavigationAspect() {
			return this;
		}

		public abstract void Present(StructuredPresenter<TreeModelNode, IPresentableItem> presenter, IPresentableItem item, TreeModelNode modelNode, PresentationState state);

		public abstract IList<string> GetQuickSearchTexts();

		bool IFileStructureAspect.CanMoveElements(RelativeLocation location, IList<CodeStructureElement> dropElements) {
			return false;
		}

		void IFileStructureAspect.MoveElements(RelativeLocation location, IList<CodeStructureElement> dropElements) {
			throw new NotSupportedException();
		}

		bool IFileStructureAspect.CanRemove() {
			return false;
		}

		void IFileStructureAspect.Remove() {
			throw new NotSupportedException();
		}

		bool IFileStructureAspect.CanRename() {
			return false;
		}

		string IFileStructureAspect.InitialName() {
			throw new NotSupportedException();
		}

		void IFileStructureAspect.Rename(string newName) {
			throw new NotSupportedException();
		}

		public virtual DocumentRange NavigationRange {
			get {
				T treeNode = GetTreeNode();
				return treeNode != null ? treeNode.GetNavigationRange() : DocumentRange.InvalidRange;
			}
		}

		bool IFileStructureAspect.InitiallyExpanded {
			get { return true; }
		}

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