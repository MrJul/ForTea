using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Impl.Caches2.WordIndex;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	/// <summary>
	/// Implementation of <see cref="IWordIndexLanguageProvider"/>.
	/// </summary>
	internal sealed class T4WordIndexProvider : IWordIndexLanguageProvider {
		
		public bool IsIdentifierFirstLetter(char ch) {
			return ch.IsLetterFast();
		}

		public bool IsIdentifierSecondLetter(char ch) {
			return ch.IsLetterOrDigitFast();
		}

		public bool CaseSensitiveIdentifiers {
			get { return false; }
		}

	}

}