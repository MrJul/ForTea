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
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Modules.ExternalFileModules;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// A component that manages <see cref="IDocument"/>s for files outside the solution.
	/// </summary>
	[SolutionComponent]
	internal class T4OutsideSolutionSourceFileManager : IPsiModuleFactory {

		private readonly StrongToWeakDictionary<FileSystemPath, IPsiSourceFile> _sourceFiles = new StrongToWeakDictionary<FileSystemPath, IPsiSourceFile>();
		private readonly IProjectFileExtensions _projectFileExtensions;
		private readonly PsiProjectFileTypeCoordinator _psiProjectFileTypeCoordinator;
		private readonly DocumentManager _documentManager;
		private readonly IPsiModule _psiModule;

		public IEnumerable<IPsiModule> Modules {
			get { return new[] { _psiModule }; }
		}

		public IPsiSourceFile GetOrCreateSourceFile([NotNull] FileSystemPath path) {
			Assertion.Assert(path.IsAbsolute, "path.IsAbsolute");
			
			lock (_sourceFiles) {
				IPsiSourceFile sourceFile;
				if (!_sourceFiles.TryGetValue(path, out sourceFile) || sourceFile == null) {
					sourceFile = new T4OutsideSolutionSourceFile(
						_projectFileExtensions, _psiProjectFileTypeCoordinator, _psiModule, path,
						sf => sf.Location.ExistsFile, sf => new T4OutsideSolutionSourceFileProperties(),
						_documentManager, EmptyResolveContext.Instance);
					_sourceFiles[path] = sourceFile;
				}
				return sourceFile;
			}
		}

		public bool HasSourceFile([NotNull] FileSystemPath path) {
			lock (_sourceFiles)
				return _sourceFiles.ContainsKey(path);
		}

		public void DeleteSourceFile([NotNull] FileSystemPath path) {
			lock (_sourceFiles)
				_sourceFiles.Remove(path);
		}

		public T4OutsideSolutionSourceFileManager([NotNull] Lifetime lifetime, [NotNull] IProjectFileExtensions projectFileExtensions,
			[NotNull] PsiProjectFileTypeCoordinator psiProjectFileTypeCoordinator, [NotNull] DocumentManager documentManager,
			[NotNull] ISolution solution) {
			_projectFileExtensions = projectFileExtensions;
			_psiProjectFileTypeCoordinator = psiProjectFileTypeCoordinator;
			_documentManager = documentManager;
			_psiModule = new PsiModuleOnFileSystemPaths(solution, "T4OutsideSolution");
			lifetime.AddDispose(_sourceFiles);
		}

	}

}