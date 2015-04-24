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
using GammaJul.ReSharper.ForTea.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Tree {

	public static class T4TreeExtensions {

		[CanBeNull]
		public static IT4Token GetAttributeValueToken([CanBeNull] this IT4Directive directive, [CanBeNull] string attributeName) {
			if (directive == null || String.IsNullOrEmpty(attributeName))
				return null;

			IT4DirectiveAttribute attribute = directive.GetAttribute(attributeName);
			if (attribute == null)
				return null;

			return attribute.GetValueToken();
		}

		[CanBeNull]
		public static string GetAttributeValue([CanBeNull] this IT4Directive directive, [CanBeNull] string attributeName) {
			IT4Token token = directive.GetAttributeValueToken(attributeName);
			return token != null ? token.GetText() : null;
		}

		public static Pair<IT4Token, string> GetAttributeValueIgnoreOnlyWhitespace([CanBeNull] this IT4Directive directive, [CanBeNull] string attributeName) {
			IT4Token valueToken = directive.GetAttributeValueToken(attributeName);
			if (valueToken == null)
				return new Pair<IT4Token, string>();

			string value = valueToken.GetText();
			if (value.Trim().Length == 0)
				return new Pair<IT4Token, string>();

			return new Pair<IT4Token, string>(valueToken, value);
		}

		public static bool IsSpecificDirective([CanBeNull] this IT4Directive directive, [CanBeNull] DirectiveInfo directiveInfo) {
			return directive != null
				&& directiveInfo != null
				&& directiveInfo.Name.Equals(directive.GetName(), StringComparison.OrdinalIgnoreCase);
		}

		[NotNull]
		public static IEnumerable<IT4Directive> GetDirectives([NotNull] this IT4DirectiveOwner directiveOwner, [NotNull] DirectiveInfo directiveInfo) {
			if (directiveOwner == null)
				throw new ArgumentNullException("directiveOwner");
			if (directiveInfo == null)
				throw new ArgumentNullException("directiveInfo");

			return directiveOwner.GetDirectives().Where(d => directiveInfo.Name.Equals(d.GetName(), StringComparison.OrdinalIgnoreCase));
		}

		[CanBeNull]
		private static string GetSortValue([NotNull] IT4Directive directive, [CanBeNull] DirectiveInfo directiveInfo,
			[NotNull] DirectiveInfoManager directiveInfoManager) {
			if (directiveInfo == directiveInfoManager.Assembly)
				return directive.GetAttributeValue(directiveInfoManager.Assembly.NameAttribute.Name);
			if (directiveInfo == directiveInfoManager.Import)
				return directive.GetAttributeValue(directiveInfoManager.Import.NamespaceAttribute.Name);
			if (directiveInfo == directiveInfoManager.Parameter)
				return directive.GetAttributeValue(directiveInfoManager.Parameter.NameAttribute.Name);
			return null;
		}

		/// <summary>
		/// Finds an anchor for a newly created directive inside a list of existing directives.
		/// </summary>
		/// <param name="newDirective">The directive to add.</param>
		/// <param name="existingDirectives">The existing directives.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		/// <returns>A pair indicating the anchor (can be null) and its relative position.</returns>
		public static Pair<IT4Directive, BeforeOrAfter> FindAnchor([NotNull] this IT4Directive newDirective, [NotNull] IT4Directive[] existingDirectives,
			[NotNull] DirectiveInfoManager directiveInfoManager) {
			if (newDirective == null)
				throw new ArgumentNullException("newDirective");
			if (existingDirectives == null)
				throw new ArgumentNullException("existingDirectives");
			if (directiveInfoManager == null)
				throw new ArgumentNullException("directiveInfoManager");

			// no anchor
			if (existingDirectives.Length == 0)
				return Pair.Of((IT4Directive) null, BeforeOrAfter.Before);

			// directive name should never be null, but you never know
			string newName = newDirective.GetName();
			if (String.IsNullOrEmpty(newName))
				return Pair.Of(existingDirectives.Last(), BeforeOrAfter.After);

			var lastDirectiveByName = new Dictionary<string, IT4Directive>(StringComparer.OrdinalIgnoreCase);
			DirectiveInfo directiveInfo = directiveInfoManager.GetDirectiveByName(newName);
			string newsortValue = GetSortValue(newDirective, directiveInfo, directiveInfoManager);

			foreach (IT4Directive existingDirective in existingDirectives) {
				string existingName = existingDirective.GetName();
				if (existingName == null)
					continue;

				lastDirectiveByName[existingName] = existingDirective;

				// directive of the same type as the new one:
				// if the new directive comes alphabetically before the existing one, we got out anchor
				if (String.Equals(existingName, newName, StringComparison.OrdinalIgnoreCase)) {
					string existingSortValue = GetSortValue(existingDirective, directiveInfo, directiveInfoManager);
					if (String.Compare(newsortValue, existingSortValue, StringComparison.OrdinalIgnoreCase) < 0)
						return Pair.Of(existingDirective, BeforeOrAfter.Before);
				}
			}

			// no anchor being alphabetically after the new directive was found:
			// the last directive of the same type will be used as an anchor
			IT4Directive lastDirective;
			if (lastDirectiveByName.TryGetValue(newName, out lastDirective))
				return Pair.Of(lastDirective, BeforeOrAfter.After);
			
			// there was no directive of the same type as the new one
			// the anchor will be the last directive of the type just before (determined by the position in DirectiveInfo.AllDirectives)
			if (directiveInfo != null) {
				int index = directiveInfoManager.AllDirectives.IndexOf(directiveInfo) - 1;
				while (index >= 0) {
					if (lastDirectiveByName.TryGetValue(directiveInfoManager.AllDirectives[index].Name, out lastDirective))
						return Pair.Of(lastDirective, BeforeOrAfter.After);
					--index;
				}
				return Pair.Of(existingDirectives.First(), BeforeOrAfter.Before);
			}

			// we don't know the directive name (shouldn't happen), use the last directive as an anchor
			return Pair.Of(existingDirectives.Last(), BeforeOrAfter.After);
		}

		[NotNull]
		public static IEnumerable<IT4Include> GetRecursiveIncludes([NotNull] this IT4IncludeOwner owner) {
			foreach (IT4Include include in owner.GetIncludes()) {
				yield return include;
				foreach (IT4Include recursiveInclude in GetRecursiveIncludes(include))
					yield return recursiveInclude;
			}
		}

		/// <summary>
		/// Gets a T4 block containing a specified C# node.
		/// </summary>
		/// <typeparam name="T">The type of expected T4 container node.</typeparam>
		/// <param name="cSharpNode">The C# node whose T4 container will be retrieved.</param>
		/// <returns>An instance of <see cref="T"/>, or <c>null</c> if no container for <paramref name="cSharpNode"/> can be found.</returns>
		[CanBeNull]
		internal static T GetT4ContainerFromCSharpNode<T>([CanBeNull] this ITreeNode cSharpNode)
		where T : ITreeNode {
			if (cSharpNode == null)
				return default(T);

			var cSharpFile = cSharpNode.GetContainingFile() as IFileImpl;
			if (cSharpFile == null || cSharpFile.SecondaryRangeTranslator == null)
				return default(T);

			DocumentRange range = cSharpNode.GetDocumentRange();
			if (!range.IsValid())
				return default (T);

			ITreeNode t4Node = cSharpFile.SecondaryRangeTranslator.OriginalFile.FindNodeAt(range);
			if (t4Node == null)
				return default(T);

			return t4Node.GetContainingNode<T>(true);
			
		}

		/// <summary>
		/// Adds a directive to a <see cref="IT4File"/> at an optimal location in the directive list.
		/// </summary>
		/// <param name="t4File">The <see cref="IT4File"/> to add the directive to.</param>
		/// <param name="directive">The directive to add.</param>
		/// <param name="directiveInfoManager">A <see cref="DirectiveInfoManager"/> used to determine the best location of the directive.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		public static IT4Directive AddDirective([NotNull] this IT4File t4File, [NotNull] IT4Directive directive, [NotNull] DirectiveInfoManager directiveInfoManager) {
			Pair<IT4Directive, BeforeOrAfter> anchor = directive.FindAnchor(t4File.GetDirectives().ToArray(), directiveInfoManager);

			if (anchor.First == null)
				return t4File.AddDirective(directive);

			return anchor.Second == BeforeOrAfter.Before
				? t4File.AddDirectiveBefore(directive, anchor.First)
				: t4File.AddDirectiveAfter(directive, anchor.First);
		}

	}

}