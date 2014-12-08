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
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Provides <see cref="T4PsiModule"/> for T4 files opened inside the solution.
	/// </summary>
	internal sealed class T4ProjectPsiModuleHandler : DelegatingProjectPsiModuleHandler {

		private readonly T4PsiModuleProvider _t4PsiModuleProvider;
		
		public override IList<IPsiModule> GetAllModules() {
			var modules = new List<IPsiModule>(base.GetAllModules());
			modules.AddRange(_t4PsiModuleProvider.GetModules());
			return modules;
		}
		
		public override void OnProjectFileChanged(IProjectFile projectFile, FileSystemPath oldLocation, PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder) {
			if (!_t4PsiModuleProvider.OnProjectFileChanged(projectFile, ref changeType, changeBuilder))
				base.OnProjectFileChanged(projectFile, oldLocation, changeType, changeBuilder);
		}

		public override IEnumerable<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile) {
			return base.GetPsiSourceFilesFor(projectFile).Concat(_t4PsiModuleProvider.GetPsiSourceFilesFor(projectFile));
		}
		
		public T4ProjectPsiModuleHandler(Lifetime lifetime, [NotNull] IProjectPsiModuleHandler handler, [NotNull] ChangeManager changeManager,
			[NotNull] T4Environment t4Environment, [NotNull] IProject project)
			: base(handler) {
			_t4PsiModuleProvider = new T4PsiModuleProvider(lifetime, project.Locks, changeManager, t4Environment);
		}

	}

}