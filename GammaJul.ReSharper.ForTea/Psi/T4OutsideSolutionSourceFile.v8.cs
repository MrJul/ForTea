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


using System;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal sealed class T4OutsideSolutionSourceFile : NavigateablePsiSourceFileWithLocation, IPsiSourceFile {

		public new IDocument Document {
			get {
				IDocument document = base.Document;
				document.SetOutsideSolutionPath(Location);
				return document;
			}
		}

		public T4OutsideSolutionSourceFile(IProjectFileExtensions projectFileExtensions,
			PsiProjectFileTypeCoordinator projectFileTypeCoordinator, IPsiModule module, FileSystemPath path,
			Func<PsiSourceFileFromPath, bool> validityCheck, Func<PsiSourceFileFromPath, IPsiSourceFileProperties> propertiesFactory,
			DocumentManager documentManager, IModuleReferenceResolveContext resolveContext)
			: base(projectFileExtensions, projectFileTypeCoordinator, module, path, validityCheck, propertiesFactory, documentManager, resolveContext) {
		}
		
	}

}