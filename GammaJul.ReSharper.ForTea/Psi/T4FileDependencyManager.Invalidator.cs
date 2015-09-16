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
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	partial class T4FileDependencyManager {

		private sealed class Invalidator : IT4FileDependencyInvalidator {

			[NotNull] private readonly HashSet<FileSystemPath> _committedFilePaths = new HashSet<FileSystemPath>();
			[NotNull] private readonly T4FileDependencyManager _fileDependencyManager;
			[NotNull] private readonly IPsiServices _psiServices;

			public void AddCommittedFilePath(FileSystemPath path) {
				_committedFilePaths.Add(path);
			}

			public void CommitNeededDocuments() {
				if (_committedFilePaths.Count == 0)
					return;

				bool markedAsDirty = false;

				// Mark includers file as dirty if their included files have changed.
				foreach (FileSystemPath committedFilePath in _committedFilePaths.ToArray()) {
					foreach (FileSystemPath includer in _fileDependencyManager.GetIncluders(committedFilePath)) {
						if (_committedFilePaths.Add(includer)) {
							foreach (IProjectItem includerProjectItem in _psiServices.Solution.FindProjectItemsByLocation(includer)) {
								var includerProjectFile = includerProjectItem as IProjectFile;
								if (includerProjectFile != null) {
									_psiServices.MarkAsDirty(includerProjectFile);
									markedAsDirty = true;
								}
							}
						}
					}
				}

				// Re-commit all documents again if needed, we need a clean state here.
				if (markedAsDirty)
					_psiServices.Files.CommitAllDocuments();
			}

			public Invalidator([NotNull] T4FileDependencyManager fileDependencyManager, IPsiServices psiServices) {
				_fileDependencyManager = fileDependencyManager;
				_psiServices = psiServices;
			}

		}

	}

	

}