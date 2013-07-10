using System;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4CSharpCustomModificationHandler {

		/// <summary>
		/// Handles the addition of an import directive
		/// </summary>
		/// <param name="psiServices">The PSI services.</param>
		/// <param name="action">The action to perform to add the directive.</param>
		/// <param name="generatedAnchor">The existing using anchor.</param>
		/// <param name="before">Whether to add the statements before of after <paramref name="generatedAnchor"/>.</param>
		/// <param name="generatedFile">The generated file.</param>
		/// <returns>An instance of <see cref="IUsingDirective"/>.</returns>
		public IUsingDirective HandleAddImport(IPsiServices psiServices, Func<IUsingDirective> action, IUsingDirective generatedAnchor, bool before, IFile generatedFile) {
			return (IUsingDirective) HandleAddImportInternal(psiServices, action, generatedAnchor, before, CSharpLanguage.Instance, generatedFile);
		}

		public bool PreferQualifiedReference(IQualifiableReference reference) {
			return reference.GetTreeNode().GetSettingsStore().GetValue(CSharpUsingSettingsAccessor.PreferQualifiedReference);
		}

		public string GetSpecialElementType(DeclaredElementPresenterStyle presenter, IDeclaredElement declaredElement, ISubstitution substitution) {
			return null;
		}

	}

}