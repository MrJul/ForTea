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
using JetBrains.Text;

namespace GammaJul.ReSharper.ForTea.Parsing {
	
	/// <summary>
	/// Factory creating <see cref="T4Lexer"/>.
	/// </summary>
	internal sealed class T4LexerFactory : ILexerFactory {

		/// <summary>
		/// Gets an unique instance of <see cref="T4LexerFactory"/>.
		/// </summary>
		internal static readonly T4LexerFactory Instance = new T4LexerFactory();

		/// <summary>
		/// Creates a new lexer for a specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer onto which the lexer operates.</param>
		/// <returns>A new lexer.</returns>
		public ILexer CreateLexer(IBuffer buffer) {
			return new T4Lexer(buffer);
		}

		private T4LexerFactory() {
		}
	}

}