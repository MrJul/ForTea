using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.UI.Controls.TreeView;
using JetBrains.Application.UI.Controls.Utils;
using JetBrains.Application.UI.TreeModels;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Services.CodeStructure {

	/// <summary>
	/// We can't inherit from CSharpCodeStructureAspect since it's internal, so we have to duplicate a bit of functionality from R# here.
	/// </summary>
	internal sealed class T4CSharpCodeStructureAspects : CodeStructureDeclarationAspects {

		[NotNull] private readonly T4CSharpCodeStructureDeclaredElement _element;

		public override bool InitiallyExpanded
			=> _element.InitiallyExpanded;

		protected override IList<string> CalculateQuickSearchTexts(IDeclaration declaration) {
			if (!declaration.IsValid())
				return EmptyList<string>.InstanceList;

			switch (declaration) {
				case IInterfaceQualificationOwner owner when owner.InterfaceQualificationReference != null:
					return new[] { owner.GetDeclaredShortName(), owner.InterfaceQualificationReference.ShortName };
				case IConstructorDeclaration constructorDeclaration:
					return new[] { constructorDeclaration.DeclaredName, "new", "ctor" };
				case IIndexerDeclaration indexerDeclaration:
					return new[] { indexerDeclaration.DeclaredName, "this" };
				case IDestructorDeclaration destructorDeclaration:
					return new[] { destructorDeclaration.DeclaredName, "Finalize" };
				case IOperatorDeclaration operatorDeclaration:
					return new[] { operatorDeclaration.DeclaredName, "operator" };
				case IEventDeclaration eventDeclaration:
					return new[] { eventDeclaration.DeclaredName, "event" };
				default:
					return base.CalculateQuickSearchTexts(declaration);
			}
		}

		public override void Present(
			StructuredPresenter<TreeModelNode, IPresentableItem> presenter,
			IPresentableItem item,
			TreeModelNode modelNode,
			PresentationState state
		) {
			base.Present(presenter, item, modelNode, state);

			if (_element.InheritanceInformation != null)
				item.Images.Add(_element.InheritanceInformation.Image, _element.InheritanceInformation.ToolTip);
			// if the children have inheritance information, we must add en empty inheritance icon so that the text is aligned
			else if (_element.Parent is T4CSharpCodeStructureDeclaredElement structureDeclaredElement && structureDeclaredElement.ChildrenWithInheritance)
				item.Images.Add(PsiServicesThemedIcons.Empty.Id);
		}
		
		public override DocumentRange[] GetNavigationRanges() {
			IDeclaration declaration = Declaration;
			if (!declaration.IsValid())
				return EmptyArray<DocumentRange>.Instance;

			if (declaration is IClassLikeDeclaration || declaration is INamespaceDeclaration) {
				return new[] {
					declaration.GetNavigationRange(),
					declaration.GetLastTokenIn().GetNavigationRange()
				};
			}

			return base.GetNavigationRanges();
		}

		public T4CSharpCodeStructureAspects([NotNull] T4CSharpCodeStructureDeclaredElement element, [NotNull] IDeclaration declaration)
			: base(declaration) {
			_element = element;
		}

	}

}