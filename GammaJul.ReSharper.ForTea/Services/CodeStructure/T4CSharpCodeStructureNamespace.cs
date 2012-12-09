using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	internal sealed class T4CSharpCodeStructureNamespace : T4CSharpCodeStructureDeclaredElement {

		public override IMemberNavigationAspect GetMemberNavigationAspect() {
			return null;
		}

		public T4CSharpCodeStructureNamespace(CodeStructureElement parentElement, IDeclaration declaration, CSharpCodeStructureProcessingState state)
			: base(parentElement, declaration, state) {
		}

	}

}