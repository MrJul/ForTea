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
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a T4 block.
	/// </summary>
	public interface IT4Block : IT4TreeNode {
		
		/// <summary>
		/// Gets the start token of the block.
		/// </summary>
		/// <returns>A block start token.</returns>
		[NotNull]
		IT4Token GetStartToken();

		/// <summary>
		/// Gets the end token of the block.
		/// </summary>
		/// <returns>A block end token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetEndToken();
 
	}

}