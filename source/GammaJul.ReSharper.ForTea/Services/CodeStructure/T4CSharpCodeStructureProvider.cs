using System.Diagnostics.CodeAnalysis;
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

		[NotNull] private readonly DirectiveInfoManager _directiveInfoManager;

		public CodeStructureRootElement Build(IPsiSourceFile sourceFile, CodeStructureOptions options) {
			if (!(sourceFile.GetTheOnlyPsiFile(T4Language.Instance) is IT4File t4File))
				return null;

			var cSharpFile = sourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) as ICSharpFile;

			var secondaryRangeTranslator = (cSharpFile as IFileImpl)?.SecondaryRangeTranslator;
			if (secondaryRangeTranslator == null)
				return null;

			var state = new CSharpCodeStructureProcessingState(CodeStructureOptions.Default);

			var rootElement = new T4CodeStructureRootElement(t4File);
			foreach (ITreeNode treeNode in t4File.Children())
				ProcessT4Node(treeNode, rootElement, cSharpFile, secondaryRangeTranslator, state);
			return rootElement;
		}

		private void ProcessT4Node(
			[NotNull] ITreeNode node,
			[NotNull] CodeStructureElement parentElement,
			[NotNull] ICSharpFile cSharpFile,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator,
			[NotNull] CSharpCodeStructureProcessingState state
		) {
			InterruptableActivityCookie.CheckAndThrow();

			switch (node) {
				case IT4Directive directive:
					ProcessT4Directive(directive, parentElement);
					return;
				case T4FeatureBlock featureBlock:
					ProcessT4FeatureBlock(featureBlock, parentElement, cSharpFile, secondaryRangeTranslator, state);
					break;
			}
		}

		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		private void ProcessT4Directive([NotNull] IT4Directive directive, [NotNull] CodeStructureElement parentElement)
			=> new T4CodeStructureDirective(parentElement, directive, _directiveInfoManager);

		private static void ProcessT4FeatureBlock(
			[NotNull] T4FeatureBlock featureBlock,
			[NotNull] CodeStructureElement parentElement,
			[NotNull] ICSharpFile cSharpFile,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator,
			[NotNull] CSharpCodeStructureProcessingState state
		) {
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

			switch (node) {
				case IDeclaration declaration:
					if (!declaration.IsSynthetic())
						ProcessCSharpDeclaration(declaration, parentElement, state);
					return;
				case IMultipleDeclaration multiDeclaration:
					ProcessCSharpMultipleDeclaration(multiDeclaration, parentElement, state);
					return;
				case IPreprocessorDirective preprocessorDirective:
					ProcessCSharpPreprocessorDirective(preprocessorDirective, parentElement, state);
					break;
			}
		}

		private static void ProcessCSharpChildren(
			[NotNull] ITreeNode node,
			[NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state
		) {
			foreach (ITreeNode childNode in node.Children())
				ProcessCSharpNode(childNode, parentElement, state);
		}

		private static void ProcessCSharpDeclaration(
			[NotNull] IDeclaration declaration,
			[NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state
		) {
			switch (declaration) {
				
				case IClassLikeDeclaration classLikeDeclaration: {
					var codeStructureClass = new T4CSharpCodeStructureDeclaredElement(parentElement, declaration, state);
					if (classLikeDeclaration.Body != null)
						ProcessCSharpChildren(classLikeDeclaration.Body, codeStructureClass, state);
					return;
				}

				case ICSharpNamespaceDeclaration namespaceDeclaration: {
					var structureNamespace = new T4CSharpCodeStructureNamespace(parentElement, declaration, state);
					if (namespaceDeclaration.Body != null)
						ProcessCSharpChildren(namespaceDeclaration.Body, structureNamespace, state);
					return;
				}

				case IAccessorDeclaration _:
					return;

				case IEnumDeclaration enumDeclaration: {
					var codeStructureElement = new T4CSharpCodeStructureDeclaredElement(parentElement, declaration, state) { InitiallyExpanded = false };
					ProcessCSharpChildren(enumDeclaration.EnumBody, codeStructureElement, state);
					return;
				}

				default: {
					var codeStructureElement2 = new T4CSharpCodeStructureDeclaredElement(parentElement, declaration, state);
					ProcessCSharpChildren(declaration, codeStructureElement2, state);
					break;
				}
			}
		}

		private static void ProcessCSharpMultipleDeclaration(
			[NotNull] IMultipleDeclaration declaration,
			[NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state
		) {
			foreach (IMultipleDeclarationMember declarationMember in declaration.Declarators) {
				if (!declarationMember.IsSynthetic())
					ProcessCSharpDeclaration(declarationMember, parentElement, state);
			}
		}

		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		private static void ProcessCSharpPreprocessorDirective(
			[NotNull] IPreprocessorDirective preprocessorDirective,
			[NotNull] CodeStructureElement parentElement,
			[NotNull] CSharpCodeStructureProcessingState state
		) {
			switch (preprocessorDirective.Kind) {

				case PreprocessorDirectiveKind.REGION:
					state.Regions.Push(new T4CSharpCodeStructureRegion(parentElement, preprocessorDirective, state));
					break;

				case PreprocessorDirectiveKind.ENDREGION:
					state.Regions.TryPop();
					new T4CSharpCodeStructureRegionEnd(parentElement, preprocessorDirective, state);
					break;
			}
		}

		public T4CSharpCodeStructureProvider([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}