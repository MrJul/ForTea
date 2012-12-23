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
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Services.Selection {

	public class T4NodeSelection : TreeNodeSelection<IT4File> {

		public override ISelectedRange Parent {
			get {
				ITreeNode parentNode = TreeNode.Parent;
				return parentNode == null ? null : new T4NodeSelection(FileNode, parentNode);
			}
		}

		public T4NodeSelection(IT4File fileNode, ITreeNode node)
			: base(fileNode, node) {
		}

	}

}