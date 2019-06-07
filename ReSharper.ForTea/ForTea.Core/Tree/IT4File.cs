using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Tree {

	/// <summary>Represents a T4 file.</summary>
	[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
	public interface IT4File : IFileImpl, IT4IncludeOwner, IT4DirectiveOwner {

		/// <summary>Gets a list of included file system paths in the file.</summary>
		/// <returns>
		/// A collection of <see cref="FileSystemPath"/>,
		/// where every element is guaranteed to be non <c>null</c> and non <see cref="FileSystemPath.IsEmpty"/>.
		/// </returns>
		[NotNull]
		[ItemNotNull]
		IEnumerable<FileSystemPath> GetNonEmptyIncludePaths();

		/// <summary>Gets a list of statement blocks contained in the file.</summary>
		/// <returns>A collection of <see cref="T4StatementBlock"/>.</returns>
		[NotNull]
		[ItemNotNull]
		IEnumerable<T4StatementBlock> GetStatementBlocks();

		/// <summary>Gets a list of feature blocks contained in the file.</summary>
		/// <returns>A collection of <see cref="T4FeatureBlock"/>.</returns>
		[NotNull]
		[ItemNotNull]
		IEnumerable<T4FeatureBlock> GetFeatureBlocks();
		
		/// <summary>Adds a new directive before an existing one.</summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed before.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		IT4Directive AddDirectiveBefore([NotNull] IT4Directive directive, [NotNull] IT4Directive anchor);

		/// <summary>Adds a new directive after an existing one.</summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed after.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		IT4Directive AddDirectiveAfter([NotNull] IT4Directive directive, [NotNull] IT4Directive anchor);

		/// <summary>Adds a new directive.</summary>
		/// <param name="directive">The directive to add.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		IT4Directive AddDirective([NotNull] IT4Directive directive);

		/// <summary>Removes a directive.</summary>
		/// <param name="directive">The directive to remove.</param>
		void RemoveDirective([CanBeNull] IT4Directive directive);

		/// <summary>Adds a new statement block.</summary>
		/// <param name="statementBlock">The statement block to add.</param>
		/// <returns>A new instance of <see cref="T4StatementBlock"/>, representing <paramref name="statementBlock"/> in the T4 file.</returns>
		[NotNull]
		T4StatementBlock AddStatementBlock([NotNull] T4StatementBlock statementBlock);

		/// <summary>Adds a new feature block.</summary>
		/// <param name="featureBlock">The feature block to add.</param>
		/// <returns>A new instance of <see cref="T4FeatureBlock"/>, representing <paramref name="featureBlock"/> in the T4 file.</returns>
		[NotNull]
		T4FeatureBlock AddFeatureBlock([NotNull] T4FeatureBlock featureBlock);

		/// <summary>Removes a child node from the file.</summary>
		/// <param name="node">The node to remove.</param>
		void RemoveChild([CanBeNull] IT4TreeNode node);

	}

}