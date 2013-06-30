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
using JetBrains.DocumentManagers;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal partial class T4PsiModule {

		[CanBeNull]
		private IProject GetProjectByOutputAssembly([NotNull] IAssembly assembly) {
			return _outputAssemblies.GetProjectByOutputAssembly(assembly);
		}

		public IEnumerable<IPsiModuleReference> GetReferences(IModuleReferenceResolveContext moduleReferenceResolveContext) {
			return GetReferences();
		}
		
		ICollection<PreProcessingDirective> IPsiModule.GetAllDefines() {
			return EmptyList<PreProcessingDirective>.InstanceList;
		}

		[NotNull]
		private PsiProjectFile CreateSourceFile([NotNull] IProjectFile projectFile, [NotNull] DocumentManager documentManager) {
			return new PsiProjectFile(
				this,
				projectFile,
				(pf, sf) => new DefaultPsiProjectFileProperties(pf, sf),
				JetFunc<IProjectFile, IPsiSourceFile>.True,
				documentManager,
				projectFile.GetProject().GetResolveContext());
		}


		[CanBeNull]
		private IAssemblyCookie CreateCookieCore([NotNull] AssemblyReferenceTarget target) {
			var resolveContext = UniversalModuleReferenceContext.Instance;
			FileSystemPath result = ResolveManager.Resolve(target, _resolveProject, resolveContext);
			return result != null
				? _assemblyFactory.AddRef(result, "T4", resolveContext)
				: null;
		}
		
		private static PsiModuleChange.ChangeType ModifiedChangeType {
			get { return PsiModuleChange.ChangeType.Modified; }
		}

	}

}