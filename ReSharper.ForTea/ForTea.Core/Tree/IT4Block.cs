using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree {

	/// <summary>Represents a T4 block.</summary>
	public interface IT4Block : IT4TreeNode {
		
		/// <summary>Gets the start token of the block.</summary>
		/// <returns>A block start token.</returns>
		[NotNull]
		IT4Token GetStartToken();

		/// <summary>Gets the end token of the block.</summary>
		/// <returns>A block end token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetEndToken();
 
	}

}