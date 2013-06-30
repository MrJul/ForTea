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

using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Implementation of <see cref="IT4Token"/>.
	/// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial class T4Token : BindedToBufferLeafElement, IT4Token {

		public override PsiLanguageType Language {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public TokenNodeType GetTokenType() {
			return (TokenNodeType) NodeType;
		}

		/// <summary>
		/// Returns a textual representation of the token.
		/// </summary>
		/// <returns>A textual representation of the token.</returns>
		public override string ToString() {
			return NodeType + ": " + GetText();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4Token"/> class.
		/// </summary>
		/// <param name="nodeType">The token type.</param>
		/// <param name="buffer">The buffer holding the token text.</param>
		/// <param name="startOffset">The token starting offset in <paramref name="buffer"/>.</param>
		/// <param name="endOffset">The token ending offset in <paramref name="buffer"/>.</param>
		public T4Token(T4TokenNodeType nodeType, IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
			: base(nodeType, buffer, startOffset, endOffset) {
		}

	}

}