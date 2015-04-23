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
using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Text;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Implementation of <see cref="IT4File"/>.
	/// </summary>
	internal sealed class T4File : FileElementBase, IT4File {

		public override PsiLanguageType Language {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public override NodeType NodeType {
			get { return T4ElementTypes.T4File; }
		}

		/// <summary>
		/// Gets a list of direct includes.
		/// </summary>
		/// <returns>A list of <see cref="IT4Include"/>.</returns>
		public IEnumerable<IT4Include> GetIncludes() {
			return this.Children<IT4Include>();
		}

		public IEnumerable<FileSystemPath> GetNonEmptyIncludePaths() {
			return GetIncludes()
				.Select(include => include.Path)
				.Where(path => path != null && !path.IsEmpty);
		}

		/// <summary>
		/// Gets a list of directives contained in the file.
		/// </summary>
		/// <returns>A collection of <see cref="IT4Directive"/>.</returns>
		public IEnumerable<IT4Directive> GetDirectives() {
			return this.Children<IT4Directive>();
		}

		/// <summary>
		/// Gets a list of statement blocks contained in the file.
		/// </summary>
		/// <returns>A collection of <see cref="T4StatementBlock"/>.</returns>
		public IEnumerable<T4StatementBlock> GetStatementBlocks() {
			return this.Children<T4StatementBlock>();
		}

		/// <summary>
		/// Gets a list of feature blocks contained in the file.
		/// </summary>
		/// <returns>A collection of <see cref="T4FeatureBlock"/>.</returns>
		public IEnumerable<T4FeatureBlock> GetFeatureBlocks() {
			return this.Children<T4FeatureBlock>();
		}

		/// <summary>
		/// Adds a new directive before an existing one.
		/// </summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed before.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		public IT4Directive AddDirectiveBefore(IT4Directive directive, IT4Directive anchor) {
			if (directive == null)
				throw new ArgumentNullException("directive");
			if (anchor == null)
				throw new ArgumentNullException("anchor");

			using (this.CreateWriteLock()) {
				directive = ModificationUtil.AddChildBefore(anchor, directive);

				// if the directive was inserted between a new line (or the file start) and the anchor, add another new line after
				// the directive so that both directives have new lines after them
				if (directive.PrevSibling == null || directive.PrevSibling.GetTokenType() == T4TokenNodeTypes.NewLine)
					ModificationUtil.AddChildAfter(directive, T4TokenNodeTypes.NewLine.CreateLeafElement());

				return directive;
			}
		}

		/// <summary>
		/// Adds a new directive after an existing one.
		/// </summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed after.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		public IT4Directive AddDirectiveAfter(IT4Directive directive, IT4Directive anchor) {
			if (directive == null)
				throw new ArgumentNullException("directive");
			if (anchor == null)
				throw new ArgumentNullException("anchor");

			using (this.CreateWriteLock()) {
				directive = ModificationUtil.AddChildAfter(anchor, directive);

				// if the directive was inserted between the anchor and a new line, add another new line before
				// the directive so that both directives have new lines after them
				ITreeNode sibling = directive.NextSibling;
				if (sibling is IT4Include)
					sibling = sibling.NextSibling;
				if (sibling != null && sibling.GetTokenType() == T4TokenNodeTypes.NewLine)
					ModificationUtil.AddChildBefore(directive, T4TokenNodeTypes.NewLine.CreateLeafElement());

				return directive;
			}
		}

		/// <summary>
		/// Adds a new directive.
		/// </summary>
		/// <param name="directive">The directive to add.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		public IT4Directive AddDirective(IT4Directive directive) {
			if (directive == null)
				throw new ArgumentNullException("directive");

			IT4Directive anchor = GetDirectives().LastOrDefault();
			if (anchor != null)
				return AddDirectiveAfter(directive, anchor);
			
			using (this.CreateWriteLock()) {
				directive = FirstChild != null
					? ModificationUtil.AddChildBefore(FirstChild, directive)
					: ModificationUtil.AddChild(this, directive);
				ModificationUtil.AddChildAfter(directive, T4TokenNodeTypes.NewLine.CreateLeafElement());
				return directive;
			}
		}

		/// <summary>
		/// Removes a directive.
		/// </summary>
		/// <param name="directive">The directive to remove.</param>
		public void RemoveDirective(IT4Directive directive) {
			if (directive == null)
				return;

			using (this.CreateWriteLock()) {
				ITreeNode endNode = directive;

				// remove the included node with the include directive
				if (directive.IsSpecificDirective(Shell.Instance.GetComponent<DirectiveInfoManager>().Include)) {
					if (directive.NextSibling is IT4Include)
						endNode = directive.NextSibling;
				}

				// remove the optional end line after the directive
				if (endNode.NextSibling != null && endNode.NextSibling.GetTokenType() == T4TokenNodeTypes.NewLine)
					endNode = endNode.NextSibling;

				ModificationUtil.DeleteChildRange(directive, endNode);
			}
		}

		/// <summary>
		/// Adds a new statement block.
		/// </summary>
		/// <param name="statementBlock">The statement block to add.</param>
		/// <returns>A new instance of <see cref="T4StatementBlock"/>, representing <paramref name="statementBlock"/> in the T4 file.</returns>
		public T4StatementBlock AddStatementBlock(T4StatementBlock statementBlock) {
			if (statementBlock == null)
				throw new ArgumentNullException("statementBlock");

			T4StatementBlock anchor = GetStatementBlocks().LastOrDefault();
			using (this.CreateWriteLock()) {
				return anchor == null
					? ModificationUtil.AddChild(this, statementBlock)
					: ModificationUtil.AddChildAfter(anchor, statementBlock);
			}
		}

		/// <summary>
		/// Adds a new feature block.
		/// </summary>
		/// <param name="featureBlock">The feature block to add.</param>
		/// <returns>A new instance of <see cref="T4FeatureBlock"/>, representing <paramref name="featureBlock"/> in the T4 file.</returns>
		public T4FeatureBlock AddFeatureBlock(T4FeatureBlock featureBlock) {
			if (featureBlock == null)
				throw new ArgumentNullException("featureBlock");

			T4FeatureBlock anchor = GetFeatureBlocks().LastOrDefault();
			using (this.CreateWriteLock()) {
				return anchor == null
					? ModificationUtil.AddChild(this, featureBlock)
					: ModificationUtil.AddChildAfter(anchor, featureBlock);
			}
		}

		/// <summary>
		/// Removes a child node from the file.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		public void RemoveChild(IT4TreeNode node) {
			if (node == null)
				return;

			using (this.CreateWriteLock())
				ModificationUtil.DeleteChild(node);
		}

		/// <summary>
		/// Obtains the caching lexer on file text
		/// </summary>
		public new CachingLexer CachingLexer {
			get {
				// TODO: this is a workaround: sometimes, TodoManager will ask for the CachingLexer
				// but the TokenBuffer is null because there was an uncommitted modification.
				// The base implementation of CachingLexer will create a TokenBuffer from the whole file,
				// including T4Includes which aren't normally included in the token buffer:
				// the next reparse will just be totally wrong.
				// See http://youtrack.jetbrains.com/issue/RSRP-331994 for a similar problem.
				TokenBuffer tokenBuffer = TokenBuffer;
				if (tokenBuffer != null)
					return tokenBuffer.CreateLexer();
				return new TokenBuffer(new T4Lexer(new StringBuffer(String.Empty))).CreateLexer();
			}
		}

	}

}