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
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Implementation of <see cref="IT4CodeBlock"/>.
	/// </summary>
	public abstract class T4CodeBlock : T4CompositeElement, IT4CodeBlock {

		/// <summary>
		/// Gets the role of a child node.
		/// </summary>
		/// <param name="nodeType">The type of the child node</param>
		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			if (nodeType == T4TokenNodeTypes.Code)
				return T4TokenRole.Code;
			if (nodeType == T4TokenNodeTypes.BlockEnd)
				return T4TokenRole.BlockEnd;
			if (nodeType == StartTokenNodeType)
				return T4TokenRole.BlockStart;
			return T4TokenRole.Unknown;
		}

		/// <summary>
		/// Gets the type of starting token.
		/// </summary>
		protected abstract TokenNodeType StartTokenNodeType { get; }

		/// <summary>
		/// Gets the text of the code block.
		/// </summary>
		/// <returns>The code text, or <c>null</c> if none is available.</returns>
		public string GetCodeText() {
			IT4Token token = GetCodeToken();
			return token != null ? token.GetText() : null;
		}

		/// <summary>
		/// Gets the start token of the block.
		/// </summary>
		/// <returns>A block start token.</returns>
		public IT4Token GetStartToken() {
			return (IT4Token) this.GetChildByRole((short) T4TokenRole.BlockStart);
		}

		/// <summary>
		/// Gets the end token of the block.
		/// </summary>
		/// <returns>A block end token, or <c>null</c> if none is available.</returns>
		public IT4Token GetEndToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.BlockEnd);
		}

		/// <summary>
		/// Gets the code token.
		/// </summary>
		/// <returns>The code token, or <c>null</c> if none is available.</returns>
		public IT4Token GetCodeToken() {
			return (IT4Token) FindChildByRole((short) T4TokenRole.Code);
		}

	}

}