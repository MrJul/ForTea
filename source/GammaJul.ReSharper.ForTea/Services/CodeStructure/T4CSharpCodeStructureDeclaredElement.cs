using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	/// <summary>We can't inherit from CSharpCodeStructureDeclaredElement since it's internal, so we have to duplicate a bit of functionality from R# here.</summary>
	internal class T4CSharpCodeStructureDeclaredElement : CodeStructureDeclarationElement {

		[CanBeNull] private readonly CSharpCodeStructureRegion _parentRegion;
		[CanBeNull] private readonly T4CSharpCodeStructureAspects _aspects;

		public override ICodeStructureBlockStart ParentBlock
			=> _parentRegion;

		public InheritanceInformation InheritanceInformation { get; }

		public bool InitiallyExpanded { get; set; }

		public bool ChildrenWithInheritance { get; private set; }

		public override IFileStructureAspect GetFileStructureAspect()
			=> _aspects;

		public override IGotoFileMemberAspect GetGotoMemberAspect()
			=> _aspects;

		public override IMemberNavigationAspect GetMemberNavigationAspect()
			=> _aspects;

		public T4CSharpCodeStructureDeclaredElement(
			[NotNull] CodeStructureElement parentElement,
			[NotNull] IDeclaration declaration,
			[NotNull] CSharpCodeStructureProcessingState state
		)
			: base(parentElement, declaration) {

			IDeclaredElement declaredElement = declaration.DeclaredElement;
			InitiallyExpanded = true;

			if (declaredElement != null && state.Options.BuildInheritanceInformation) {
				InheritanceInformation = InheritanceInformation.FromDeclaredElement(declaredElement);
				if (InheritanceInformation != null) {
					if (parentElement is T4CSharpCodeStructureDeclaredElement structureDeclaredElement)
						structureDeclaredElement.ChildrenWithInheritance = true;
				}
			}

			_parentRegion = state.Regions.TryPeek();

			if (declaredElement != null)
				_aspects = new T4CSharpCodeStructureAspects(this, declaration);
		}

	}

}