using System.Collections.Generic;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a T4 directive, like assembly or import.
	/// </summary>
	public interface IT4Directive : IT4Block, IT4NamedNode {

		/// <summary>
		/// Returns the attributes of the directive.
		/// </summary>
		[NotNull]
		IEnumerable<IT4DirectiveAttribute> GetAttributes();

		/// <summary>
		/// Returns an attribute that has a given name.
		/// </summary>
		/// <param name="name">The name of the attribute to retrieve. The search is case-insensitive.</param>
		/// <returns>An instance of <see cref="IT4DirectiveAttribute"/>, or <c>null</c> if none had the name <paramref name="name"/>.</returns>
		[CanBeNull]
		IT4DirectiveAttribute GetAttribute([CanBeNull] string name);

	}

}