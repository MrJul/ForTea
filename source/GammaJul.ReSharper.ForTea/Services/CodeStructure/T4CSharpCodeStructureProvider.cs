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
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4CSharpCodeStructureProvider : IProjectFileCodeStructureProvider {

		private readonly DirectiveInfoManager _directiveInfoManager;

		public CodeStructureRootElement Build(IPsiSourceFile sourceFile, CodeStructureOptions options) {
			var t4File = sourceFile.GetTheOnlyPsiFile(T4Language.Instance) as IT4File;
			if (t4File == null)
				return null;

			// TODO: handle VB
			var cSharpFile = sourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) as ICSharpFile;
			var cSharpFileImpl = cSharpFile as IFileImpl;
			if (cSharpFileImpl == null || cSharpFileImpl.SecondaryRangeTranslator == null)
				return null;

			var state = new CSharpCodeStructureProcessingState(CodeStructureOptions.Default);

			var rootElement = new T4CodeStructureRootElement(t4File);
			foreach (ITreeNode treeNode in t4File.Children())
				ProcessT4Node(treeNode, rootElement, cSharpFile, cSharpFileImpl.SecondaryRangeTranslator, state);
			return rootElement;
		}

		private void ProcessT4Node([NotNull] ITreeNode node, [NotNull] CodeStructureElement parentElement, [NotNull] ICSharpFile cSharpFile,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator, [NotNull] CSharpCodeStructureProcessingState state) {
			InterruptableActivityCookie.CheckAndThrow();


			var directive = node as IT4Directive;
			if (directive != null) {
				ProcessT4Directive(directive, parentElement);
				return;
			}

			var featureBlock = node as T4FeatureBlock;
			if (featureBlock != null)
				ProcessT4FeatureBlock(featureBlock, parentElement, cSharpFile, secondaryRangeTranslator, state);
		}

		private void ProcessT4Directive([NotNull] IT4Directive directive, [NotNull] CodeStructureElement parentElement) {
			// ReSharper disable ObjectCreationAsStatement
			new T4CodeStructureDirective(parentElement, directive, _directiveInfoManager);
			// ReSharper restore ObjectCreationAsStatement
		}

		private static void ProcessT4FeatureBlock([NotNull] T4FeatureBlock featureBlock, [NotNull] CodeStructureElement parentElement,
			[NotNull] ICSharpFile cSharpFile, [NotNull] ISecondaryRangeTranslator secondaryRangeTranslator, [NotNull] CSharpCodeStructureProcessingState state) {
			TreeTextRange t4Range = featureBlock.GetCodeToken().GetTreeTextRange();
			TreeTextRange cSharpRange = secondaryRangeTranslator.OriginalToGenerated(t4Range);
			if (!cSharpRange.IsValid())
				return;

			TreeOffset cSharpStart = cSharpRange.StartOffset;
			TreeOffset cSharpEnd = cSharpRange.EndOffset;

			ITreeNode containingNode = cSharpFile.FindNodeAt(cSharpRange);
			if (containingNode == null)
				return;

			for (ITreeNode node = containingNode.FirstChild; node != null; node = node.NextSibling) {
				TreeOffset nodeStart = node.GetTreeStartOffset();
				if (nodeStart >= cSharpStart && nodeStart < cSharpEnd)
					ProcessCSharpNode(node, parentElement, state);
			}
		}

		private static void ProcessCSharpNode([NotNull] ITreeNode node, [NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state) {
			InterruptableActivityCookie.CheckAndThrow();

			var declaration = node as IDeclaration;
			if (declaration != null) {
				if (!declaration.IsSynthetic())
					ProcessCSharpDeclaration(declaration, parentElement, state);
				return;
			}

			var multiDeclaration = node as IMultipleDeclaration;
			if (multiDeclaration != null) {
				ProcessCSharpMultipleDeclaration(multiDeclaration, parentElement, state);
				return;
			}

			var preprocessorDirective = node as IPreprocessorDirective;
			if (preprocessorDirective != null)
				ProcessCSharpPreprocessorDirective(preprocessorDirective, parentElement, state);
		}

		private static void ProcessCSharpChildren([NotNull] ITreeNode node, [NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state) {
			foreach (ITreeNode childNode in node.Children())
				ProcessCSharpNode(childNode, parentElement, state);
		}

		private static void ProcessCSharpDeclaration([NotNull] IDeclaration declaration, [NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state) {
			var classLikeDeclaration = declaration as IClassLikeDeclaration;
			if (classLikeDeclaration != null) {
				var codeStructureClass = new T4CSharpCodeStructureDeclaredElement(parentElement, declaration, state);
				if (classLikeDeclaration.Body != null)
					ProcessCSharpChildren(classLikeDeclaration.Body, codeStructureClass, state);
				return;
			}

			var namespaceDeclaration = declaration as ICSharpNamespaceDeclaration;
			if (namespaceDeclaration != null) {
				var structureNamespace = new T4CSharpCodeStructureNamespace(parentElement, declaration, state);
				if (namespaceDeclaration.Body != null)
					ProcessCSharpChildren(namespaceDeclaration.Body, structureNamespace, state);
				return;
			}

			if (declaration is IAccessorDeclaration)
				return;

			var codeStructureElement = new T4CSharpCodeStructureDeclaredElement(parentElement, declaration, state);
			var enumDeclaration = declaration as IEnumDeclaration;
			if (enumDeclaration != null) {
				codeStructureElement.InitiallyExpanded = false;
				ProcessCSharpChildren(enumDeclaration.EnumBody, codeStructureElement, state);
				return;
			}

			ProcessCSharpChildren(declaration, codeStructureElement, state);
		}

		private static void ProcessCSharpMultipleDeclaration([NotNull] IMultipleDeclaration declaration, [NotNull] CodeStructureElement parentElement,
			CSharpCodeStructureProcessingState state) {
			foreach (IMultipleDeclarationMember declarationMember in declaration.Declarators) {
				if (!declarationMember.IsSynthetic())
					ProcessCSharpDeclaration(declarationMember, parentElement, state);
			}
		}


		private static void ProcessCSharpPreprocessorDirective([NotNull] IPreprocessorDirective preprocessorDirective,
			[NotNull] CodeStructureElement parentElement, [NotNull] CSharpCodeStructureProcessingState state) {
			switch (preprocessorDirective.Kind) {

				case PreprocessorDirectiveKind.REGION:
					state.Regions.Push(new T4CSharpCodeStructureRegion(parentElement, preprocessorDirective, state));
					break;

				case PreprocessorDirectiveKind.ENDREGION:
					state.Regions.TryPop();
					// ReSharper disable ObjectCreationAsStatement
					new T4CSharpCodeStructureRegionEnd(parentElement, preprocessorDirective, state);
					// ReSharper restore ObjectCreationAsStatement
					break;
			}
		}

		public T4CSharpCodeStructureProvider([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}