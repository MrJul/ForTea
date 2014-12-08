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
using JetBrains.Util;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
#if RS90
using JetBrains.Application.changes;
#endif

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Provides <see cref="T4PsiModule"/> for T4 files opened outside of the solution.
	/// </summary>
	[MiscFilesProjectPsiModuleProvider]
	public sealed class T4MiscFilesProjectPsiModuleProvider : IMiscFilesProjectPsiModuleProvider {

		private readonly T4PsiModuleProvider _t4PsiModuleProvider;
		
		public IEnumerable<IPsiModule> GetModules() {
			return _t4PsiModuleProvider.GetModules();
		}

		public IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile) {
			return _t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile);
		}

		public void Dispose() {
			_t4PsiModuleProvider.Dispose();
		}

		public void OnProjectFileChanged(IProjectFile projectFile, PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder, FileSystemPath oldLocation) {
			_t4PsiModuleProvider.OnProjectFileChanged(projectFile, ref changeType, changeBuilder);
		}

		public T4MiscFilesProjectPsiModuleProvider([NotNull] Lifetime lifetime, [NotNull] IShellLocks shellLocks,
			[NotNull] ChangeManager changeManager, [NotNull] T4Environment t4Environment) {
			_t4PsiModuleProvider = new T4PsiModuleProvider(lifetime, shellLocks, changeManager, t4Environment);
		}

	}

}