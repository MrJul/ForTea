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
using JetBrains.DataFlow;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// A component that manages <see cref="IDocument"/>s for files outside the solution.
	/// </summary>
	[SolutionComponent]
	internal class T4OutsideSolutionSourceFileManager {

		private readonly StrongToWeakDictionary<FileSystemPath, IPsiSourceFile> _sourceFiles = new StrongToWeakDictionary<FileSystemPath, IPsiSourceFile>();
		private readonly IDocumentFactory _documentFactory;
		private readonly IProjectFileExtensions _projectFileExtensions;
		private readonly PsiProjectFileTypeCoordinator _psiProjectFileTypeCoordinator;
		
		public IPsiSourceFile GetOrCreateSourceFile([NotNull] FileSystemPath path) {
			Assertion.Assert(path.IsAbsolute, "path.IsAbsolute");
			
			lock (_sourceFiles) {
				IPsiSourceFile sourceFile;
				if (!_sourceFiles.TryGetValue(path, out sourceFile) || sourceFile == null) {
					sourceFile = new T4OutsideSolutionSourceFile(
						_documentFactory, _projectFileExtensions, _psiProjectFileTypeCoordinator, null, path,
						sf => sf.Path.ExistsFile, sf => new T4OutsideSolutionSourceFileProperties());
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

		public T4OutsideSolutionSourceFileManager([NotNull] Lifetime lifetime, [NotNull] IDocumentFactory documentFactory, [NotNull] IProjectFileExtensions projectFileExtensions,
			[NotNull] PsiProjectFileTypeCoordinator psiProjectFileTypeCoordinator) {
			_documentFactory = documentFactory;
			_projectFileExtensions = projectFileExtensions;
			_psiProjectFileTypeCoordinator = psiProjectFileTypeCoordinator;
			lifetime.AddDispose(_sourceFiles);
		}

	}

}