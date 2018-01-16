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
using System.Collections.Generic;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// A T4 include. This is not a directive, it contains the included file tree.
	/// </summary>
	internal sealed class T4Include : T4CompositeElement, IT4Include {

		public override NodeType NodeType {
			get { return T4ElementTypes.T4Include; }
		}

		public FileSystemPath Path { get; set; }

		/// <summary>
		/// Gets a list of direct includes.
		/// </summary>
		/// <returns>A list of <see cref="IT4Include"/>.</returns>
		public IEnumerable<IT4Include> GetIncludes() {
			return this.Children<IT4Include>();
		}

		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			return T4TokenRole.Unknown;
		}

		public IEnumerable<IT4Directive> GetDirectives() {
			return this.Children<IT4Directive>();
		}

		public IDocumentRangeTranslator DocumentRangeTranslator { get; set; }

	}

}