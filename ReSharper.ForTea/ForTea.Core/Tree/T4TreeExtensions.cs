using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Tree {

	public static class T4TreeExtensions {

		[CanBeNull]
		public static IT4Token GetAttributeValueToken([CanBeNull] this IT4Directive directive, [CanBeNull] string attributeName) {
			if (String.IsNullOrEmpty(attributeName))
				return null;

			return directive?.GetAttribute(attributeName)?.GetValueToken();
		}

		[CanBeNull]
		public static string GetAttributeValue([CanBeNull] this IT4Directive directive, [CanBeNull] string attributeName)
			=> directive.GetAttributeValueToken(attributeName)?.GetText();

		public static Pair<IT4Token, string> GetAttributeValueIgnoreOnlyWhitespace([CanBeNull] this IT4Directive directive, [CanBeNull] string attributeName) {
			IT4Token valueToken = directive.GetAttributeValueToken(attributeName);
			if (valueToken == null)
				return new Pair<IT4Token, string>();

			string value = valueToken.GetText();
			if (value.Trim().Length == 0)
				return new Pair<IT4Token, string>();

			return new Pair<IT4Token, string>(valueToken, value);
		}

		public static bool IsSpecificDirective([CanBeNull] this IT4Directive directive, [CanBeNull] DirectiveInfo directiveInfo)
			=> directive != null
			&& directiveInfo != null
			&& directiveInfo.Name.Equals(directive.GetName(), StringComparison.OrdinalIgnoreCase);

		[NotNull]
		public static IEnumerable<IT4Directive> GetDirectives([NotNull] this IT4DirectiveOwner directiveOwner, [NotNull] DirectiveInfo directiveInfo)
			=> directiveOwner.GetDirectives().Where(d => directiveInfo.Name.Equals(d.GetName(), StringComparison.OrdinalIgnoreCase));

		[CanBeNull]
		private static string GetSortValue(
			[NotNull] IT4Directive directive,
			[CanBeNull] DirectiveInfo directiveInfo,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		) {
			if (directiveInfo == directiveInfoManager.Assembly)
				return directive.GetAttributeValue(directiveInfoManager.Assembly.NameAttribute.Name);
			if (directiveInfo == directiveInfoManager.Import)
				return directive.GetAttributeValue(directiveInfoManager.Import.NamespaceAttribute.Name);
			if (directiveInfo == directiveInfoManager.Parameter)
				return directive.GetAttributeValue(directiveInfoManager.Parameter.NameAttribute.Name);
			return null;
		}

		/// <summary>Finds an anchor for a newly created directive inside a list of existing directives.</summary>
		/// <param name="newDirective">The directive to add.</param>
		/// <param name="existingDirectives">The existing directives.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="T4DirectiveInfoManager"/>.</param>
		/// <returns>A pair indicating the anchor (can be null) and its relative position.</returns>
		public static Pair<IT4Directive, BeforeOrAfter> FindAnchor(
			[NotNull] this IT4Directive newDirective,
			[NotNull] IT4Directive[] existingDirectives,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		) {

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
			if (lastDirectiveByName.TryGetValue(newName, out IT4Directive lastDirective))
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
		[ItemNotNull]
		public static IEnumerable<IT4Include> GetRecursiveIncludes([NotNull] this IT4IncludeOwner owner) {
			foreach (IT4Include include in owner.GetIncludes()) {
				yield return include;
				foreach (IT4Include recursiveInclude in GetRecursiveIncludes(include))
					yield return recursiveInclude;
			}
		}

		/// <summary>Gets a T4 block containing a specified C# node.</summary>
		/// <typeparam name="T">The type of expected T4 container node.</typeparam>
		/// <param name="cSharpNode">The C# node whose T4 container will be retrieved.</param>
		/// <returns>An instance of <see cref="T"/>, or <c>null</c> if no container for <paramref name="cSharpNode"/> can be found.</returns>
		[CanBeNull]
		public static T GetT4ContainerFromCSharpNode<T>([CanBeNull] this ITreeNode cSharpNode)
		where T : ITreeNode {
			ISecondaryRangeTranslator secondaryRangeTranslator = (cSharpNode?.GetContainingFile() as IFileImpl)?.SecondaryRangeTranslator;
			if (secondaryRangeTranslator == null)
				return default;

			DocumentRange range = cSharpNode.GetDocumentRange();
			if (!range.IsValid())
				return default;

			ITreeNode t4Node = secondaryRangeTranslator.OriginalFile.FindNodeAt(range);
			if (t4Node == null)
				return default;

			return t4Node.GetContainingNode<T>(true);
			
		}

		/// <summary>Adds a directive to a <see cref="IT4File"/> at an optimal location in the directive list.</summary>
		/// <param name="t4File">The <see cref="IT4File"/> to add the directive to.</param>
		/// <param name="directive">The directive to add.</param>
		/// <param name="directiveInfoManager">A <see cref="T4DirectiveInfoManager"/> used to determine the best location of the directive.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		public static IT4Directive AddDirective(
			[NotNull] this IT4File t4File,
			[NotNull] IT4Directive directive,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		) {
			(IT4Directive anchor, BeforeOrAfter beforeOrAfter) = directive.FindAnchor(t4File.GetDirectives().ToArray(), directiveInfoManager);

			if (anchor == null)
				return t4File.AddDirective(directive);

			return beforeOrAfter == BeforeOrAfter.Before
				? t4File.AddDirectiveBefore(directive, anchor)
				: t4File.AddDirectiveAfter(directive, anchor);
		}

	}

}