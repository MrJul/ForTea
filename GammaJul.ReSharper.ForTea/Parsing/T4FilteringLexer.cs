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
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Parsing {


	/// <summary>
	/// Lexer filtering whitespaces.
	/// </summary>
	internal sealed class T4FilteringLexer : FilteringLexer {

		/// <summary>
		/// Returns whether to skips a specified token type.
		/// </summary>
		/// <param name="tokenType">Type of the token.</param>
		/// <returns><c>true</c> if the token must be skipped, <c>false</c> otherwise.</returns>
		protected override bool Skip(TokenNodeType tokenType) {
			return tokenType.IsWhitespace;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4FilteringLexer"/> class.
		/// </summary>
		/// <param name="lexer">The base lexer.</param>
		internal T4FilteringLexer(ILexer lexer)
			: base(lexer) {
		}

	}

}