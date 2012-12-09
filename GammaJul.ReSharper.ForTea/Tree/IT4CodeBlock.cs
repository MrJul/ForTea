using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Contract representing a T4 code block.
	/// </summary>
	public interface IT4CodeBlock : IT4Block {

		/// <summary>
		/// Gets the text of the code block.
		/// </summary>
		/// <returns>The code text, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		string GetCodeText();

		/// <summary>
		/// Gets the code token.
		/// </summary>
		/// <returns>The code token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetCodeToken();

	}

}