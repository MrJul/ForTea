using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Tree
{
	/// <summary>epresents a T4 directive attribute, like namespace in import directive.</summary>
	public interface IT4DirectiveAttribute : IT4NamedNode
	{
		/// <summary>Gets the token representing the equal sign between the name and the value of this attribute.</summary>
		/// <returns>An equal token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetEqualToken();

		/// <summary>Gets the token representing the value of this attribute.</summary>
		/// <returns>A value token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		TreeElement GetValueToken();

		/// <summary>Gets the value of this attribute.</summary>
		/// <returns>The attribute value, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		string GetValue();

		/// <summary>Gets the error associated with the value that have been identified at parsing time.</summary>
		[CanBeNull]
		string ValueError { get; }

		[CanBeNull]
		FileSystemPath Reference { get; }
	}
}
