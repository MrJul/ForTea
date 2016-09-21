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
using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.Settings;
using JetBrains.ReSharper.Psi.CSharp.Impl.CustomHandlers;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Psi.Web.CodeBehindSupport;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// C# custom modification handler that allows the T4 files to be modified in response to C# actions or quickfixes.
	/// (eg: adding a using statement translates to an import directive).
	/// </summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4CSharpCustomModificationHandler : CustomModificationHandler<IT4CodeBlock, IT4Directive> {

		[NotNull] private readonly DirectiveInfoManager _directiveInfoManager;

		/// <summary>
		/// Determines whether namespace aliases can be used.
		/// </summary>
		/// <returns>Always <c>false</c> since T4 files does not support aliases.</returns>
		public bool CanUseAliases {
			get { return false; }
		}

		/// <summary>
		/// Determines whether static imports can be used.
		/// </summary>
		/// <returns>Always <c>false</c> since T4 files does not support static imports.</returns>
		public bool CanUseStaticImport {
			get { return false; }
		}

		public bool CanOmitBraces {
			get { return false; }
		}

		/// <summary>
		/// Creates a new T4 code block.
		/// </summary>
		/// <param name="text">The C# code.</param>
		/// <param name="anchor">Where to insert the code.</param>
		/// <returns>A new instance of <see cref="IT4CodeBlock"/>.</returns>
		protected override IT4CodeBlock CreateInlineCodeBlock(string text, ITreeNode anchor) {
			ITreeNode existingFeatureNode = anchor.FindPrevNode(node => node is T4FeatureBlock ? TreeNodeActionType.ACCEPT : TreeNodeActionType.CONTINUE);
			return existingFeatureNode != null
				? (IT4CodeBlock) T4ElementFactory.Instance.CreateFeatureBlock(text)
				: T4ElementFactory.Instance.CreateStatementBlock(text);
		}

		/// <summary>
		/// Gets the code tree text range of a code block.
		/// </summary>
		/// <param name="codeBlock">The code block.</param>
		/// <returns>A <see cref="TreeTextRange"/> representing the code range in <paramref name="codeBlock"/>.</returns>
		protected override TreeTextRange GetCodeTreeTextRange(IT4CodeBlock codeBlock) {
			IT4Token codeToken = codeBlock.GetCodeToken();
			return codeToken != null ? codeToken.GetTreeTextRange() : TreeTextRange.InvalidRange;
		}

		/// <summary>
		/// Creates a T4 import directive instead of a C# using directive.
		/// </summary>
		/// <param name="before"><c>true</c> to create the directive before <paramref name="anchor"/>; <c>false</c> to create it after.</param>
		/// <param name="anchor">An existing directive serving as an anchor for the new directive.</param>
		/// <param name="usingDirective">The C# using directive.</param>
		/// <param name="originalFile">The original T4 file where the directive must be created.</param>
		/// <returns>A <see cref="TreeTextRange"/> corresponding to the namespace in the newly created directive.</returns>
		protected override TreeTextRange CreateUsingNode(bool before, IT4Directive anchor, ITreeNode usingDirective, IFile originalFile) {
			var t4File = (IT4File) originalFile;
			string ns = GetNamespaceFromUsingDirective(usingDirective);
			IT4Directive directive = _directiveInfoManager.Import.CreateDirective(ns);

			if (anchor != null && anchor.GetContainingNode<IT4Include>() == null)
				directive = before ? t4File.AddDirectiveBefore(directive, anchor) : t4File.AddDirectiveAfter(directive, anchor);
			else
				directive = t4File.AddDirective(directive, _directiveInfoManager);

			return directive.GetAttributeValueToken(_directiveInfoManager.Import.NamespaceAttribute.Name).GetTreeTextRange();
		}

		/// <summary>
		/// Gets the text range of a C# using directive namespace.
		/// </summary>
		/// <param name="usingDirective">The using directive.</param>
		/// <returns>A <see cref="TreeTextRange"/> corresponding to the namespace in <paramref name="usingDirective"/>.</returns>
		protected override TreeTextRange GetNameRange(ITreeNode usingDirective) {
			return (usingDirective as IUsingDirective).GetUsedNamespaceNode().GetTreeTextRange();
		}

		/// <summary>
		/// Removes an import directive.
		/// </summary>
		/// <param name="originalFile">The original T4 file where the directive must be removed.</param>
		/// <param name="directiveInOriginalFile">The import directive in the file.</param>
		protected override void RemoveUsingNode(IFile originalFile, IT4Directive directiveInOriginalFile) {
			((IT4File) originalFile).RemoveDirective(directiveInOriginalFile);
		}

		/// <summary>
		/// Creates a new feature block with new type members.
		/// </summary>
		/// <param name="originalFile">The original T4 file where the feature block must be created.</param>
		/// <param name="text">The code representing new C# type members.</param>
		/// <param name="first">The first node.</param>
		/// <param name="last">The last node.</param>
		/// <returns>A <see cref="TreeTextRange"/> representing the code range in the newly created feature block.</returns>
		protected override TreeTextRange CreateTypeMemberNode(IFile originalFile, string text, ITreeNode first, ITreeNode last) {
			T4FeatureBlock featureBlock = T4ElementFactory.Instance.CreateFeatureBlock(text);
			featureBlock = ((IT4File) originalFile).AddFeatureBlock(featureBlock);
			return featureBlock.GetCodeToken().GetTreeTextRange();
		}

		/// <summary>
		/// Creates a new line token.
		/// </summary>
		/// <param name="psiModule">The associated PSI module.</param>
		/// <returns>A T4 new line token.</returns>
		protected override ITreeNode CreateNewLineToken(IPsiModule psiModule) {
			return CSharpTokenType.NEW_LINE.CreateLeafElement();
		}

		/// <summary>
		/// Gets an existing feature block that can contains type members.
		/// </summary>
		/// <param name="originalFile">The original T4 file.</param>
		/// <returns>A valid <see cref="TreeTextRange"/> if a feature block existed, <see cref="TreeTextRange.InvalidRange"/> otherwise.</returns>
		protected override TreeTextRange GetExistingTypeMembersRange(IFile originalFile) {
			T4FeatureBlock lastFeatureBlock = ((IT4File) originalFile).GetFeatureBlocks().LastOrDefault();
			return lastFeatureBlock == null
				? TreeTextRange.InvalidRange
				: lastFeatureBlock.GetCodeToken().GetTreeTextRange();
		}


		protected override void AddSuperClassDirectiveToOriginalFile(IFile originalFile, ITreeNode anchor, ITreeNode superClassGeneratedNode) {
			var t4File = (IT4File) originalFile;
			IT4Directive directive = t4File.GetDirectives(_directiveInfoManager.Template).FirstOrDefault();
			IT4DirectiveAttribute attribute;
			string superClassName = superClassGeneratedNode.GetText();

			if (directive == null) {
				directive = _directiveInfoManager.Template.CreateDirective(Pair.Of(_directiveInfoManager.Template.InheritsAttribute.Name, superClassName));
				directive = t4File.AddDirective(directive, _directiveInfoManager);
				attribute = directive.GetAttributes().First();
			}
			else {
				attribute = directive.AddAttribute(_directiveInfoManager.Template.InheritsAttribute.CreateDirectiveAttribute(superClassName));
			}

			superClassGeneratedNode.GetRangeTranslator().AddProjectionItem(
				new TreeTextRange<Generated>(superClassGeneratedNode.GetTreeTextRange()),
				new TreeTextRange<Original>(attribute.GetValueToken().GetTreeTextRange()));
		}

		protected override ITreeNode GetSuperClassNodeFromOriginalFile(IFile originalFile) {
			var t4File = (IT4File) originalFile;
			foreach (IT4Directive templateDirective in t4File.GetDirectives(_directiveInfoManager.Template)) {
				IT4Token inheritsToken = templateDirective.GetAttributeValueToken(_directiveInfoManager.Template.InheritsAttribute.Name);
				if (inheritsToken != null)
					return inheritsToken;
			}
			return null;
		}

		/// <summary>
		/// Determines whether a specified C# using directive can be removed.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="usingDirective">The using directive.</param>
		/// <returns><c>true</c> if the specified using directive can be removed; otherwise, <c>false</c>.</returns>
		/// <remarks>As long as the using is represented as a T4 import directive in the root file, it can be removed.</remarks>
		public bool CanRemoveUsing(IDocument document, IUsingDirective usingDirective) {
			TreeTextRange nameRange = GetNameRange(usingDirective);
			if (!nameRange.IsValid())
				return false;

			IFile containingFile = usingDirective.GetContainingFile();
			if (containingFile == null)
				return false;

			DocumentRange documentRange = containingFile.GetDocumentRange(nameRange);
			return documentRange.IsValid() && documentRange.Document == document;

//			IReferenceName namespaceNode = usingDirective.GetUsedNamespaceNode();
//			if (namespaceNode == null)
//				return false;
//
//			var directive = namespaceNode.GetT4ContainerFromCSharpNode<IT4Directive>();
//			return directive != null && directive.GetContainingNode<IT4Include>() == null;
		}


		
		/// <summary>
		/// Translates changes in generated code-behind file to original file.
		/// </summary>
		/// <param name="psiServices">The PSI services.</param>
		/// <param name="addAction">The action that will add C# statements.</param>
		/// <param name="block">The C# block where the statement will be inserted.</param>
		/// <param name="anchor">The anchor.</param>
		/// <param name="before">Whether to add the statements before of after <paramref name="anchor"/>.</param>
		/// <param name="strict">If true, HTML whitespace statements on bounds are not included. Use for single added statement to be returned.</param>
		/// <returns>An instance of <see cref="ICSharpStatementsRange"/>.</returns>
		public ICSharpStatementsRange HandleAddStatementsRange(IPsiServices psiServices, Func<ITreeNode, ICSharpStatementsRange> addAction, IBlock block, ITreeNode anchor, bool before, bool strict) {
			using (CustomGeneratedChangePromotionCookie.Create(block)) {
				ICSharpStatementsRange range = addAction(anchor);
				FinishAddStatementsRange(range.TreeRange, before);
				return range;
			}
		}

		public void HandleRemoveStatementsRange(IPsiServices psiServices, ITreeRange treeRange, Action action) {
			action();
		}

		public ITreeRange HandleChangeStatements(IPsiServices psiServices, ITreeRange rangeBeforeChange, Func<ITreeRange> changeAction, bool strict) {
			return changeAction();
		}

		public void HandleChangeExpressionInStatement(IPsiServices psiServices, IStatement statement, Action changeAction) {
			changeAction();
		}

		/// <summary>
		/// Handles the removal of an import directive.
		/// </summary>
		/// <param name="psiServices">The PSI services.</param>
		/// <param name="scope">The namespace scope.</param>
		/// <param name="usingDirective">The using directive to remove.</param>
		/// <param name="action">The action to perform to remove the directive.</param>
		public void HandleRemoveImport(IPsiServices psiServices, ICSharpTypeAndNamespaceHolderDeclaration scope, IUsingDirective usingDirective, Action action) {
			ICSharpTreeNode namespaceNode = usingDirective.GetUsedNamespaceNode();
			if (namespaceNode == null)
				Assertion.Fail("Only a namespace using can be removed.");
			else {
				TreeTextRange range = namespaceNode.GetTreeTextRange();
				HandleRemoveImportInternal(psiServices, scope, usingDirective, action, CSharpLanguage.Instance, range);
			}
		}

		/// <summary>
		/// Handles the removal of a type member from a code block.
		/// </summary>
		/// <param name="psiServices">The PSI services.</param>
		/// <param name="node">The node that must be removed.</param>
		/// <param name="action">The action to execute to remove the node.</param>
		public void HandleRemoveTypeMember(IPsiServices psiServices, ITreeNode node, Action action) {
			action();
			RemoveContainingBlockIfEmpty(node);
		}

		private static void RemoveContainingBlockIfEmpty([CanBeNull] ITreeNode node) {
			var block = node.GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			if (block == null)
				return;

			string code = block.GetCodeText();
			if (code == null || code.Trim().Length == 0)
				return;

			var file = block.GetContainingFile() as IT4File;
			if (file != null)
				file.RemoveChild(block);
		}

		/// <summary>
		/// Gets the body of a method that is visible for user.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>Always the body of <paramref name="method"/>.</returns>
		public IBlock GetMethodBodyVisibleForUser(ICSharpFunctionDeclaration method) {
			return method.Body;
		}

		public bool IsToAddImportsToDeepestScope(ITreeNode context) {
			return false;
		}
		
		/// <summary>
		/// Retrives the namespace from a C# using directive.
		/// </summary>
		/// <param name="usingDirective">The using directive.</param>
		/// <returns>The namespace contained in <paramref name="usingDirective"/>.</returns>
		[NotNull]
		private static string GetNamespaceFromUsingDirective([NotNull] ITreeNode usingDirective) {
			IReferenceName namespaceNode = (usingDirective as IUsingDirective).GetUsedNamespaceNode();
			if (namespaceNode == null)
				throw new FailPsiTransactionException("Cannot create namespace alias.");
			return namespaceNode.QualifiedName;
		}

		/// <summary>
		/// Handles the addition of an import directive
		/// </summary>
		/// <param name="psiServices">The PSI services.</param>
		/// <param name="action">The action to perform to add the directive.</param>
		/// <param name="generatedAnchor">The existing using anchor.</param>
		/// <param name="before">Whether to add the statements before of after <paramref name="generatedAnchor"/>.</param>
		/// <param name="generatedFile">The generated file.</param>
		/// <returns>An instance of <see cref="IUsingDirective"/>.</returns>
		public IUsingDirective HandleAddImport(IPsiServices psiServices, Func<IUsingDirective> action, ITreeNode generatedAnchor, bool before, IFile generatedFile) {
			return (IUsingDirective) HandleAddImportInternal(psiServices, action, generatedAnchor, before, CSharpLanguage.Instance, generatedFile);
		}

		public bool PreferQualifiedReference(IQualifiableReference reference) {
			return reference.GetTreeNode().GetSettingsStore().GetValue(CSharpUsingSettingsAccessor.PreferQualifiedReference);
		}

		public string GetSpecialMethodType(DeclaredElementPresenterStyle presenter, IMethod method, ISubstitution substitution) {
			return null;
		}

		public ThisQualifierSettingsKey GetThisQualifierStyle(ITreeNode context) {
			return context.GetSettingsStore().GetKey<ThisQualifierSettingsKey>(SettingsOptimization.OptimizeDefault);
		}

		public StaticQualifierSettingsKey GetStaticQualifierStyle(ITreeNode context) {
			return context.GetSettingsStore().GetKey<StaticQualifierSettingsKey>(SettingsOptimization.OptimizeDefault);
		}

		public IList<ITreeRange> GetHolderBlockRanges(ITreeNode treeNode) {
			return new ITreeRange[] {
				new TreeRange(treeNode.FirstChild, treeNode.LastChild)
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4CSharpCustomModificationHandler"/> class.
		/// </summary>
		/// <param name="languageManager">The language manager.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		public T4CSharpCustomModificationHandler([NotNull] ILanguageManager languageManager, [NotNull] DirectiveInfoManager directiveInfoManager)
			: base(languageManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}