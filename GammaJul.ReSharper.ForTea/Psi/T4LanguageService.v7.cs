using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4LanguageService {

		private static readonly T4WordIndexProvider _indexProvider = new T4WordIndexProvider();
		
		/// <summary>
		/// Gets a word index language provider for T4.
		/// </summary>
		/// <returns>An implementation of <see cref="IWordIndexLanguageProvider"/>.</returns>
		public override IWordIndexLanguageProvider WordIndexLanguageProvider {
			get { return _indexProvider; }
		}
		
		/// <summary>
		/// Determines whether a given node is filtered.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <returns><c>true</c> if <paramref name="node"/> is a whitespace; otherwise, <c>false</c>.</returns>
		public override bool IsFilteredNode(ITreeNode node) {
			TokenNodeType nodeType = node.GetTokenType();
			return nodeType != null && nodeType.IsWhitespace;
		}

	}

}