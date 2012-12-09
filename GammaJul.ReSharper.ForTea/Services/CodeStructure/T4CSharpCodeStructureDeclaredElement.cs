using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	/// <summary>
	/// We can't inherit from CSharpCodeStructureDeclaredElement since it's internal, so we have to duplicate a bit of functionality from R# here.
	/// </summary>
	internal class T4CSharpCodeStructureDeclaredElement : CodeStructureDeclarationElement {

		private readonly InheritanceInformation _inheritanceInformation;
		private readonly CSharpCodeStructureRegion _parentRegion;
		private readonly T4CSharpCodeStructureAspects _aspects;

		public override ICodeStructureBlockStart ParentBlock {
			get { return _parentRegion; }
		}

		public InheritanceInformation InheritanceInformation {
			get { return _inheritanceInformation; }
		}

		public bool InitiallyExpanded { get; set; }

		public bool ChildrenWithInheritance { get; private set; }

		public override IFileStructureAspect GetFileStructureAspect() {
			return _aspects;
		}

		public override IGotoFileMemberAspect GetGotoMemberAspect() {
			return _aspects;
		}

		public override IMemberNavigationAspect GetMemberNavigationAspect() {
			return _aspects;
		}

		public T4CSharpCodeStructureDeclaredElement(CodeStructureElement parentElement, IDeclaration declaration, CSharpCodeStructureProcessingState state)
			: base(parentElement, declaration) {
			IDeclaredElement declaredElement = declaration.DeclaredElement;
			InitiallyExpanded = true;

			if (declaredElement != null && state.Options.BuildInheritanceInformation) {
				_inheritanceInformation = InheritanceInformation.FromDeclaredElement(declaredElement);
				if (_inheritanceInformation != null) {
					var structureDeclaredElement = parentElement as T4CSharpCodeStructureDeclaredElement;
					if (structureDeclaredElement != null)
						structureDeclaredElement.ChildrenWithInheritance = true;
				}
			}

			_parentRegion = state.Regions.TryPeek();

			if (declaredElement != null)
				_aspects = new T4CSharpCodeStructureAspects(this, declaration);
		}

	}

}