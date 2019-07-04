using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Services.CodeStructure {

	internal sealed class T4CSharpCodeStructureNamespace : T4CSharpCodeStructureDeclaredElement {

		public override IMemberNavigationAspect GetMemberNavigationAspect()
			=> null;

		public T4CSharpCodeStructureNamespace(
			[NotNull] CodeStructureElement parentElement,
			[NotNull] IDeclaration declaration,
			[NotNull] CSharpCodeStructureProcessingState state
		)
			: base(parentElement, declaration, state) {
		}

	}

}