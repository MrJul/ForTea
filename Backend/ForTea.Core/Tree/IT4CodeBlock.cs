using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree {

	/// <summary>Contract representing a T4 code block.</summary>
	public interface IT4CodeBlock : IT4Block {

		/// <summary>Gets the text of the code block.</summary>
		/// <returns>The code text, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		string GetCodeText();

		/// <summary>Gets the code token.</summary>
		/// <returns>The code token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		TreeElement GetCodeToken();

	}

}