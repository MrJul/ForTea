#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
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