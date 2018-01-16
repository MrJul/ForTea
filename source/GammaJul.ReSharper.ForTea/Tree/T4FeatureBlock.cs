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
	/// Represents a feature block (&lt;#+ ... #&gt;).
	/// </summary>
	public class T4FeatureBlock : T4CodeBlock {

		/// <summary>
		/// Gets the node type of this element.
		/// </summary>
		public override NodeType NodeType {
			get { return T4ElementTypes.T4FeatureBlock; }
		}

		/// <summary>
		/// Gets the type of starting token.
		/// </summary>
		protected override TokenNodeType StartTokenNodeType {
			get { return T4TokenNodeTypes.FeatureStart; }
		}

	}

}