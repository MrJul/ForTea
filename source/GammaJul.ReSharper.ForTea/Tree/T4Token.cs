using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>Implementation of <see cref="IT4Token"/>.</summary>
	public class T4Token : BindedToBufferLeafElement, IT4Token {

		public override PsiLanguageType Language
			=> T4Language.Instance;

		/// <summary>Gets the node type of this element.</summary>
		public TokenNodeType GetTokenType()
			=> (TokenNodeType) NodeType;

		/// <summary>Checks if this element is filtered (whitespace, comment or error).</summary>
		public override bool IsFiltered()
			=> GetTokenType().IsFiltered;

		/// <summary>Returns a textual representation of the token.</summary>
		/// <returns>A textual representation of the token.</returns>
		public override string ToString()
			=> NodeType + ": " + GetText();

		/// <summary>Initializes a new instance of the <see cref="T4Token"/> class.</summary>
		/// <param name="nodeType">The token type.</param>
		/// <param name="buffer">The buffer holding the token text.</param>
		/// <param name="startOffset">The token starting offset in <paramref name="buffer"/>.</param>
		/// <param name="endOffset">The token ending offset in <paramref name="buffer"/>.</param>
		public T4Token([NotNull] T4TokenNodeType nodeType, [NotNull] IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
			: base(nodeType, buffer, startOffset, endOffset) {
		}

	}

}