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
using JetBrains.ProjectModel;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Represents a T4 project file type.
	/// </summary>
	[ProjectFileTypeDefinition(Name)]
	public class T4ProjectFileType : KnownProjectFileType {

		/// <summary>
		/// Gets an unique instance of <see cref="T4ProjectFileType"/>.
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public new static readonly T4ProjectFileType Instance;

		/// <summary>
		/// Gets the name of the file type.
		/// </summary>
		public new const string Name = T4Language.Name;

		private T4ProjectFileType()
			: base(Name, Name, new[] { ".tt", ".ttinclude", ".t4" }) {
		}

		protected T4ProjectFileType([NotNull] string name, [NotNull] string presentableName, [NotNull] IEnumerable<string> extensions)
			: base(name, presentableName, extensions) {
		}

		protected T4ProjectFileType([NotNull] string name, [NotNull] string presentableName)
			: base(name, presentableName) {
		}

		protected T4ProjectFileType([NotNull] string name)
			: base(name) {
		}

	}

}