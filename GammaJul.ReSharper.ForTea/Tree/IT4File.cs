#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a T4 file.
	/// </summary>
	public interface IT4File : IFileImpl, IT4IncludeOwner, IT4DirectiveOwner {

		/// <summary>
		/// Gets a list of included file system paths in the file.
		/// </summary>
		/// <returns>
		/// A collection of <see cref="FileSystemPath"/>,
		/// where every element is guaranteed to be non <c>null</c> and non <see cref="FileSystemPath.IsEmpty"/>.
		/// </returns>
		[NotNull]
		IEnumerable<FileSystemPath> GetNonEmptyIncludePaths();

		/// <summary>
		/// Gets a list of statement blocks contained in the file.
		/// </summary>
		/// <returns>A collection of <see cref="T4StatementBlock"/>.</returns>
		[NotNull]
		IEnumerable<T4StatementBlock> GetStatementBlocks();

		/// <summary>
		/// Gets a list of feature blocks contained in the file.
		/// </summary>
		/// <returns>A collection of <see cref="T4FeatureBlock"/>.</returns>
		[NotNull]
		IEnumerable<T4FeatureBlock> GetFeatureBlocks();
		
		/// <summary>
		/// Adds a new directive before an existing one.
		/// </summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed before.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		IT4Directive AddDirectiveBefore([NotNull] IT4Directive directive, [NotNull] IT4Directive anchor);

		/// <summary>
		/// Adds a new directive after an existing one.
		/// </summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed after.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		IT4Directive AddDirectiveAfter([NotNull] IT4Directive directive, [NotNull] IT4Directive anchor);

		/// <summary>
		/// Adds a new directive.
		/// </summary>
		/// <param name="directive">The directive to add.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		IT4Directive AddDirective([NotNull] IT4Directive directive);

		/// <summary>
		/// Removes a directive.
		/// </summary>
		/// <param name="directive">The directive to remove.</param>
		void RemoveDirective([CanBeNull] IT4Directive directive);

		/// <summary>
		/// Adds a new statement block.
		/// </summary>
		/// <param name="statementBlock">The statement block to add.</param>
		/// <returns>A new instance of <see cref="T4StatementBlock"/>, representing <paramref name="statementBlock"/> in the T4 file.</returns>
		[NotNull]
		T4StatementBlock AddStatementBlock([NotNull] T4StatementBlock statementBlock);

		/// <summary>
		/// Adds a new feature block.
		/// </summary>
		/// <param name="featureBlock">The feature block to add.</param>
		/// <returns>A new instance of <see cref="T4FeatureBlock"/>, representing <paramref name="featureBlock"/> in the T4 file.</returns>
		[NotNull]
		T4FeatureBlock AddFeatureBlock([NotNull] T4FeatureBlock featureBlock);

		/// <summary>
		/// Removes a child node from the file.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		void RemoveChild([CanBeNull] IT4TreeNode node);

	}

}