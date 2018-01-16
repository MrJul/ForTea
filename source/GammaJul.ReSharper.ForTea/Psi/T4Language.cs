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
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Represents the T4 language.
	/// </summary>
	[LanguageDefinition(Name)]
	public class T4Language : KnownLanguage {

		/// <summary>
		/// Gets the name of the T4 language.
		/// </summary>
		public new const string Name = "T4";

		/// <summary>
		/// Gets an unique instance of <see cref="T4Language"/>.
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public static T4Language Instance;

		private T4Language()
			: base(Name, Name) {
		}

		protected T4Language([NotNull] string name, [NotNull] string presentableName)
			: base(name, presentableName) {
		}

		protected T4Language([NotNull] string name)
			: base(name) {
		}

	}

}