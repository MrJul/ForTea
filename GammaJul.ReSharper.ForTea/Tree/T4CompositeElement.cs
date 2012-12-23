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
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Base class for all T4 composite elements.
	/// </summary>
	public abstract class T4CompositeElement : CompositeElement {

		public override PsiLanguageType Language {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the child role of a child element.
		/// </summary>
		/// <param name="child">The child element.</param>
		/// <returns></returns>
		public sealed override short GetChildRole(TreeElement child) {
			return (short) GetChildRole(child.NodeType);
		}

		/// <summary>
		/// Gets the role of a child node.
		/// </summary>
		/// <param name="nodeType">The type of the child node</param>
		protected abstract T4TokenRole GetChildRole([NotNull] NodeType nodeType);

	}

}