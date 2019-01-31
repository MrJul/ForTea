using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	partial class T4FileDependencyManager {

		private sealed class Invalidator : IT4FileDependencyInvalidator {

			[NotNull] [ItemNotNull] private readonly HashSet<FileSystemPath> _committedFilePaths = new HashSet<FileSystemPath>();
			[NotNull] private readonly T4FileDependencyManager _fileDependencyManager;
			[NotNull] private readonly IPsiServices _psiServices;

			public void AddCommittedFilePath(FileSystemPath path)
				=> _committedFilePaths.Add(path);

			public void CommitNeededDocuments() {
				if (_committedFilePaths.Count == 0)
					return;

				bool markedAsDirty = false;

				// Mark includers file as dirty if their included files have changed.
				foreach (FileSystemPath committedFilePath in _committedFilePaths.ToArray()) {
					foreach (FileSystemPath includer in _fileDependencyManager.GetIncluders(committedFilePath)) {
						if (!_committedFilePaths.Add(includer))
							continue;

						foreach (IProjectItem includerProjectItem in _psiServices.Solution.FindProjectItemsByLocation(includer)) {
							if (includerProjectItem is IProjectFile includerProjectFile) {
								_psiServices.MarkAsDirty(includerProjectFile);
								markedAsDirty = true;
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