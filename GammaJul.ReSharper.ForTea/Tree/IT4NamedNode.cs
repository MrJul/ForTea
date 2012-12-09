using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a T4 node that can have a name.
	/// </summary>
	public interface IT4NamedNode : IT4TreeNode {

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		/// <returns>The node name, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		string GetName();

		/// <summary>
		/// Gets the token representing the name of this node.
		/// </summary>
		/// <returns>The name token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetNameToken();

	}

}