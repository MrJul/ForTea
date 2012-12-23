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

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Represents the difference between two <see cref="T4FileData" />.
	/// </summary>
	public sealed class T4FileDataDiff {

		private readonly IEnumerable<string> _addedAssemblies;
		private readonly IEnumerable<string> _removedAssemblies;
		private readonly IEnumerable<string> _addedMacros;

			/// <summary>
		/// Gets an enumeration of all added assemblies.
		/// </summary>
		[NotNull]
		public IEnumerable<string> AddedAssemblies {
			get { return _addedAssemblies; }
		}

		/// <summary>
		/// Gets an enumeration of all removed assemblies.
		/// </summary>
		[NotNull]
		public IEnumerable<string> RemovedAssemblies {
			get { return _removedAssemblies; }
		}

		/// <summary>
		/// Gets en enumeration of all added macros.
		/// </summary>
		[NotNull]
		public IEnumerable<string> AddedMacros {
			get { return _addedMacros; }
		}

		public T4FileDataDiff([NotNull] IEnumerable<string> addedAssemblies, [NotNull] IEnumerable<string> removedAssemblies,
			[NotNull] IEnumerable<string> addedMacros) {
			_addedAssemblies = addedAssemblies;
			_removedAssemblies = removedAssemblies;
			_addedMacros = addedMacros;
		}

	}

}