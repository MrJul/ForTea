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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Language service for T4 project files.
	/// </summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public sealed class T4ProjectFileLanguageService : MixedProjectFileLanguageService {

		private readonly T4Environment _t4Environment;

		/// <summary>
		/// Gets the PSI language type, <see cref="T4Language" />.
		/// </summary>
		protected override PsiLanguageType PsiLanguageType {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the T4 file icon.
		/// TODO: add an image.
		/// </summary>
		public override IconId Icon {
			get { return null; }
		}

		/// <summary>
		/// Get the PSI properties (if any) for the specific project file
		/// </summary>
		public override IPsiSourceFileProperties GetPsiProperties(IProjectFile projectFile, IPsiSourceFile sourceFile) {
			return new T4PsiProjectFileProperties(projectFile, sourceFile, _t4Environment.IsSupported);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4ProjectFileLanguageService" /> class.
		/// </summary>
		/// <param name="t4ProjectFileType">Type of the T4 project file.</param>
		/// <param name="t4Environment">The host environment.</param>
		public T4ProjectFileLanguageService(T4ProjectFileType t4ProjectFileType, T4Environment t4Environment)
			: base(t4ProjectFileType) {
			_t4Environment = t4Environment;
		}

	}

}