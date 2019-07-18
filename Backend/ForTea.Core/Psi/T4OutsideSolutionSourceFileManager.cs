using System;
using JetBrains.Annotations;
using JetBrains.Application.FileSystemTracker;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Modules.ExternalFileModules;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace GammaJul.ForTea.Core.Psi {

	/// <summary>A component that manages <see cref="IDocument"/>s for files outside the solution.</summary>
	[SolutionComponent]
	internal class T4OutsideSolutionSourceFileManager : IPsiModuleFactory {

		[NotNull] private readonly StrongToWeakDictionary<FileSystemPath, IPsiSourceFile> _sourceFiles;
		[NotNull] private readonly IProjectFileExtensions _projectFileExtensions;
		[NotNull] private readonly PsiProjectFileTypeCoordinator _psiProjectFileTypeCoordinator;
		[NotNull] private readonly DocumentManager _documentManager;
		[NotNull] private readonly IPsiModule _psiModule;

		public HybridCollection<IPsiModule> Modules
			=> new HybridCollection<IPsiModule>(_psiModule);

		public IPsiSourceFile GetOrCreateSourceFile([NotNull] FileSystemPath path) {
			Assertion.Assert(path.IsAbsolute, "path.IsAbsolute");
			
			lock (_sourceFiles) {
				if (!_sourceFiles.TryGetValue(path, out IPsiSourceFile sourceFile) || sourceFile == null) {
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

		public T4OutsideSolutionSourceFileManager(
			Lifetime lifetime,
			[NotNull] IProjectFileExtensions projectFileExtensions,
			[NotNull] PsiProjectFileTypeCoordinator psiProjectFileTypeCoordinator,
			[NotNull] DocumentManager documentManager,
			[NotNull] ISolution solution,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IFileSystemTracker fileSystemTracker
		) {
			_projectFileExtensions = projectFileExtensions;
			_psiProjectFileTypeCoordinator = psiProjectFileTypeCoordinator;
			_documentManager = documentManager;
			_sourceFiles = new StrongToWeakDictionary<FileSystemPath, IPsiSourceFile>(lifetime);
			_psiModule = new PsiModuleOnFileSystemPaths(
				solution,
				"T4OutsideSolution",
				Guid.NewGuid().ToString(),
				t4Environment.TargetFrameworkId,
				fileSystemTracker,
				lifetime,
				false);
			lifetime.OnTermination(_sourceFiles);
		}

	}

}
