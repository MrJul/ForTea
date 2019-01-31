using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;

namespace GammaJul.ReSharper.ForTea.Tree {

	public interface IT4IncludeOwner : ICompositeElement, IT4TreeNode {

		/// <summary>Gets a list of direct includes.</summary>
		/// <returns>A list of <see cref="IT4Include"/>.</returns>
		[NotNull]
		[ItemNotNull]
		IEnumerable<IT4Include> GetIncludes();

		[CanBeNull]
		IDocumentRangeTranslator DocumentRangeTranslator { get; }

	}

}