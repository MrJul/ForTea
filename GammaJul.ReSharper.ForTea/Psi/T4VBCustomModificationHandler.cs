#region License

//    Copyright 2012 Julien Lebosquain
//    Copyright 2016 Caelan Sayler - [caelantsayler]at[gmail]com
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
using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB;
using JetBrains.ReSharper.Psi.VB.Impl.CustomHandlers;
using JetBrains.ReSharper.Psi.VB.Parsing;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.Web.CodeBehindSupport;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// VB custom modification handler that allows the T4 files to be modified in response to VB actions or quickfixes.
	/// (eg: adding a VB Import statement translates to a T4 import directive).
	/// </summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4VBCustomModificationHandler : CustomModificationHandler<IT4CodeBlock, IT4Directive>, IVBCustomModificationHandler {

		[NotNull] private readonly DirectiveInfoManager _directiveInfoManager;

		public T4VBCustomModificationHandler([NotNull] ILanguageManager languageManager, [NotNull] DirectiveInfoManager directiveInfoManager)
			: base(languageManager) {
			_directiveInfoManager = directiveInfoManager;
		}

		public bool CanRemoveUsing(IDocument document, IImportDirective usingDirective) {
			var nameRange = GetNameRange(usingDirective);
			if (!nameRange.IsValid())
				return false;

			var containingFile = usingDirective.GetContainingFile();
			if (containingFile == null)
				return false;

			var documentRange = containingFile.GetDocumentRange(nameRange);
			return documentRange.IsValid() && documentRange.Document == document;
		}

		public void HandleRemoveStatementsRange(IPsiServices psiServices, ITreeRange treeRange, Action action) {
			action();
		}

		public IVBStatementsRange HandleAddStatementsRange(IPsiServices psiServices, Func<IVBStatementsRange> addAction, bool before) {
			var transaction = psiServices.Transactions;
			using (CustomGeneratedChangePromotionCookie.Create<VBLanguage>(transaction)) {
				var range = addAction();
				FinishAddStatementsRange(range.TreeRange, before);
				return range;
			}
		}

		public void HandleRemoveImport(IPsiServices psiServices, IVBTypeAndNamespaceHolderDeclaration scope, IImportDirective usingDirective,
			Action action) {
			var range = usingDirective.GetTreeTextRange();
			HandleRemoveImportInternal(psiServices, scope, usingDirective, action, VBLanguage.Instance, range);
		}

		public IImportDirective HandleAddImport(IPsiServices psiServices, Func<IImportDirective> action, ITreeNode generatedAnchor, bool before,
			IFile generatedFile) {
			return (IImportDirective) HandleAddImportInternal(psiServices, action, generatedAnchor, before, VBLanguage.Instance, generatedFile);
		}

		public bool CanUseAliases
		{
			get { return false; }
		}

		protected override IT4CodeBlock CreateInlineCodeBlock(string text, ITreeNode anchor) {
			var existingFeatureNode = anchor.FindPrevNode(node => node is T4FeatureBlock ? TreeNodeActionType.ACCEPT : TreeNodeActionType.CONTINUE);
			return existingFeatureNode != null
				? (IT4CodeBlock) T4ElementFactory.Instance.CreateFeatureBlock(text)
				: T4ElementFactory.Instance.CreateStatementBlock(text);
		}

		protected override TreeTextRange GetCodeTreeTextRange(IT4CodeBlock codeBlock) {
			var codeToken = codeBlock.GetCodeToken();
			return codeToken != null ? codeToken.GetTreeTextRange() : TreeTextRange.InvalidRange;
		}

		protected override TreeTextRange GetNameRange(ITreeNode usingDirective) {
			return (usingDirective as IImportDirective).GetTreeTextRange();
		}

		protected override void RemoveUsingNode(IFile originalFile, IT4Directive directiveInOriginalFile) {
			((IT4File) originalFile).RemoveDirective(directiveInOriginalFile);
		}

		protected override TreeTextRange CreateTypeMemberNode(IFile originalFile, string text, ITreeNode first, ITreeNode last) {
			var featureBlock = T4ElementFactory.Instance.CreateFeatureBlock(text);
			featureBlock = ((IT4File) originalFile).AddFeatureBlock(featureBlock);
			return featureBlock.GetCodeToken().GetTreeTextRange();
		}

		protected override ITreeNode CreateNewLineToken(IPsiModule psiModule) {
			return VBTokenType.LINE_TERMINATOR.CreateLeafElement();
		}

		protected override TreeTextRange GetExistingTypeMembersRange(IFile originalFile) {
			var lastFeatureBlock = ((IT4File) originalFile).GetFeatureBlocks().LastOrDefault();
			return lastFeatureBlock == null
				? TreeTextRange.InvalidRange
				: lastFeatureBlock.GetCodeToken().GetTreeTextRange();
		}

		protected override void AddSuperClassDirectiveToOriginalFile(IFile originalFile, ITreeNode anchor, ITreeNode superClassGeneratedNode) {
			var t4File = (IT4File) originalFile;
			var directive = t4File.GetDirectives(_directiveInfoManager.Template).FirstOrDefault();
			IT4DirectiveAttribute attribute;
			var superClassName = superClassGeneratedNode.GetText();

			if (directive == null) {
				directive =
					_directiveInfoManager.Template.CreateDirective(Pair.Of(_directiveInfoManager.Template.InheritsAttribute.Name, superClassName));
				directive = t4File.AddDirective(directive, _directiveInfoManager);
				attribute = directive.GetAttributes().First();
			}
			else
				attribute = directive.AddAttribute(_directiveInfoManager.Template.InheritsAttribute.CreateDirectiveAttribute(superClassName));

			superClassGeneratedNode.GetRangeTranslator().AddProjectionItem(
				new TreeTextRange<Generated>(superClassGeneratedNode.GetTreeTextRange()),
				new TreeTextRange<Original>(attribute.GetValueToken().GetTreeTextRange()));
		}

		protected override ITreeNode GetSuperClassNodeFromOriginalFile(IFile originalFile) {
			var t4File = (IT4File) originalFile;
			foreach (var templateDirective in t4File.GetDirectives(_directiveInfoManager.Template)) {
				var inheritsToken = templateDirective.GetAttributeValueToken(_directiveInfoManager.Template.InheritsAttribute.Name);
				if (inheritsToken != null)
					return inheritsToken;
			}
			return null;
		}

		protected override TreeTextRange CreateUsingNode(bool before, IT4Directive anchor, ITreeNode usingNode, IFile originalFile) {
			var t4File = (IT4File) originalFile;
			var ranges = new List<TreeTextRange>();

			var importDirective = (IImportDirective) usingNode;
			foreach (var claus in importDirective.ImportClauses) {
				var namespaceClaus = claus as IImportNamespaceClause;
				if (namespaceClaus == null)
					continue;
				var namespaceNode = namespaceClaus.ImportedNamespaceReferenceName;
				if (namespaceNode == null)
					continue;

				var ns = namespaceNode.QualifiedName;
				var directive = _directiveInfoManager.Import.CreateDirective(ns);
				if (anchor != null && anchor.GetContainingNode<IT4Include>() == null)
					directive = before ? t4File.AddDirectiveBefore(directive, anchor) : t4File.AddDirectiveAfter(directive, anchor);
				else
					directive = t4File.AddDirective(directive, _directiveInfoManager);

				ranges.Add(directive.GetAttributeValueToken(_directiveInfoManager.Import.NamespaceAttribute.Name).GetTreeTextRange());
			}

			var minRange = ranges.Min(m => m.GetMinOffset().Offset);
			var maxRange = ranges.Max(m => m.GetMaxOffset().Offset);
			return new TreeTextRange(new TreeOffset(minRange), new TreeOffset(maxRange));
		}

	}

}