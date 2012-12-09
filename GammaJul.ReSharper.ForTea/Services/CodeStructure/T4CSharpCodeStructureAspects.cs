using System.Collections.Generic;
using JetBrains.CommonControls;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Services.Resources;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	/// <summary>
	/// We can't inherit from CSharpCodeStructureAspect since it's internal, so we have to duplicate a bit of functionality from R# here.
	/// </summary>
	internal sealed class T4CSharpCodeStructureAspects : CodeStructureDeclarationAspects {

		private readonly T4CSharpCodeStructureDeclaredElement _element;

		public override bool InitiallyExpanded {
			get { return _element.InitiallyExpanded; }
		}

		protected override IList<string> CalculateQuickSearchTexts(IDeclaration declaration) {
			if (!declaration.IsValid())
				return EmptyList<string>.InstanceList;

			var owner = declaration as IInterfaceQualificationOwner;
			if (owner != null && owner.InterfaceQualificationReference != null)
				return new[] { owner.GetDeclaredShortName(), owner.InterfaceQualificationReference.ShortName };

			var constructorDeclaration = declaration as IConstructorDeclaration;
			if (constructorDeclaration != null)
				return new[] { constructorDeclaration.DeclaredName, "new", "ctor" };

			var indexerDeclaration = declaration as IIndexerDeclaration;
			if (indexerDeclaration != null)
				return new[] { indexerDeclaration.DeclaredName, "this" };

			var destructorDeclaration = declaration as IDestructorDeclaration;
			if (destructorDeclaration != null)
				return new[] { destructorDeclaration.DeclaredName, "Finalize" };

			var operatorDeclaration = declaration as IOperatorDeclaration;
			if (operatorDeclaration != null)
				return new[] { operatorDeclaration.DeclaredName, "operator" };

			var eventDeclaration = declaration as IEventDeclaration;
			if (eventDeclaration != null)
				return new[] { eventDeclaration.DeclaredName, "event" };

			return base.CalculateQuickSearchTexts(declaration);
		}

		public override void Present(StructuredPresenter<TreeModelNode, IPresentableItem> presenter, IPresentableItem item,
			TreeModelNode modelNode, PresentationState state) {
			base.Present(presenter, item, modelNode, state);
			if (_element.InheritanceInformation != null)
				item.Images.Add(_element.InheritanceInformation.Image, _element.InheritanceInformation.ToolTip);
			else {
				// if the children have inheritance information, we must add en empty inheritance icon so that the text is aligned
				var structureDeclaredElement = _element.Parent as T4CSharpCodeStructureDeclaredElement;
				if (structureDeclaredElement != null && structureDeclaredElement.ChildrenWithInheritance)
					item.Images.Add(PsiServicesThemedIcons.Empty.Id);
			}
		}
		
		public override DocumentRange[] GetNavigationRanges() {
			if (!Declaration.IsValid())
				return EmptyArray<DocumentRange>.Instance;

			if (Declaration is IClassLikeDeclaration || Declaration is INamespaceDeclaration) {
				return new[] {
					Declaration.GetNavigationRange(),
					Declaration.GetLastTokenIn().GetNavigationRange()
				};
			}

			return base.GetNavigationRanges();
		}

		public T4CSharpCodeStructureAspects(T4CSharpCodeStructureDeclaredElement element, IDeclaration declaration)
			: base(declaration) {
			_element = element;
		}

	}

}