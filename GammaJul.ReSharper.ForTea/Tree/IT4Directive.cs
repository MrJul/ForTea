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
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Represents a T4 directive, like assembly or import.
	/// </summary>
	public interface IT4Directive : IT4Block, IT4NamedNode {

		/// <summary>
		/// Returns the attributes of the directive.
		/// </summary>
		[NotNull]
		IEnumerable<IT4DirectiveAttribute> GetAttributes();

		/// <summary>
		/// Returns an attribute that has a given name.
		/// </summary>
		/// <param name="name">The name of the attribute to retrieve. The search is case-insensitive.</param>
		/// <returns>An instance of <see cref="IT4DirectiveAttribute"/>, or <c>null</c> if none had the name <paramref name="name"/>.</returns>
		[CanBeNull]
		IT4DirectiveAttribute GetAttribute([CanBeNull] string name);

	}

}