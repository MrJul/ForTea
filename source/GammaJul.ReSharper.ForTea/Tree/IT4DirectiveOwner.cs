using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	public interface IT4DirectiveOwner : ICompositeElement, IT4TreeNode {

		/// <summary>Gets a list of directives contained in the element.</summary>
		/// <returns>A collection of <see cref="IT4Directive"/>.</returns>
		[NotNull]
		[ItemNotNull]
		IEnumerable<IT4Directive> GetDirectives();

	}

}