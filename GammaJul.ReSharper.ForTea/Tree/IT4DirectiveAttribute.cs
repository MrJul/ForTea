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
	/// Represents a T4 directive attribute, like namespace in import directive.
	/// </summary>
	public interface IT4DirectiveAttribute : IT4NamedNode {

		/// <summary>
		/// Gets the token representing the equal sign between the name and the value of this attribute.
		/// </summary>
		/// <returns>An equal token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetEqualToken();

		/// <summary>
		/// Gets the token representing the value of this attribute.
		/// </summary>
		/// <returns>A value token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetValueToken();

		/// <summary>
		/// Gets the value of this attribute.
		/// </summary>
		/// <returns>The attribute value, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		string GetValue();

		/// <summary>
		/// Gets the error associated with the value that have been identified at parsing time.
		/// </summary>
		[CanBeNull]
		string ValueError { get; }

	}

}