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
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Contains the different types of T4 composite elements.
	/// </summary>
	public static class T4ElementTypes {

		public static readonly CompositeNodeType T4File = new T4CompositeNodeType<T4File>();
		public static readonly CompositeNodeType T4StatementBlock = new T4CompositeNodeType<T4StatementBlock>();
		public static readonly CompositeNodeType T4ExpressionBlock = new T4CompositeNodeType<T4ExpressionBlock>();
		public static readonly CompositeNodeType T4FeatureBlock = new T4CompositeNodeType<T4FeatureBlock>();
		public static readonly CompositeNodeType T4Directive = new T4CompositeNodeType<T4Directive>();
		public static readonly CompositeNodeType T4DirectiveAttribute = new T4CompositeNodeType<T4DirectiveAttribute>();
		public static readonly CompositeNodeType T4Include = new T4CompositeNodeType<T4Include>();

		private class T4CompositeNodeType<T> : CompositeNodeType
		where T : CompositeElement, new() {
			
			public override CompositeElement Create() {
				return new T();
			}

			internal T4CompositeNodeType()
				: base(typeof(T).Name) {
			}

		}

	}

}