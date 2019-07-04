using System.Collections.Generic;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree {

	/// <summary>Represents a T4 directive, like assembly or import.</summary>
	public interface IT4Directive : IT4Block, IT4NamedNode {

		/// <summary>Returns the attributes of the directive.</summary>
		[NotNull]
		IEnumerable<IT4DirectiveAttribute> GetAttributes();

		/// <summary>Returns an attribute that has a given name.</summary>
		/// <param name="name">The name of the attribute to retrieve. The search is case-insensitive.</param>
		/// <returns>An instance of <see cref="IT4DirectiveAttribute"/>, or <c>null</c> if none had the name <paramref name="name"/>.</returns>
		[CanBeNull]
		IT4DirectiveAttribute GetAttribute([CanBeNull] string name);

		/// <summary>Adds a new attribute to this directive.</summary>
		/// <param name="attribute">The attribute to add.</param>
		/// <returns>A new instance of <see cref="IT4DirectiveAttribute"/>, representing <paramref name="attribute"/> in the T4 file.</returns>
		[NotNull]
		IT4DirectiveAttribute AddAttribute([NotNull] IT4DirectiveAttribute attribute);

	}

}