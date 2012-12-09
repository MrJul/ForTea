using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	/// <summary>
	/// Manages <see cref="T4PsiModule"/> for T4 files.
	/// Contains common implementation for <see cref="T4ProjectPsiModuleHandler"/> and <see cref="T4MiscFilesProjectPsiModuleProvider"/>.
	/// </summary>
	[SolutionComponent]
	public sealed class T4PsiModuleProvider : IDisposable {
		private readonly Dictionary<IProjectFile, ModuleWrapper> _modules = new Dictionary<IProjectFile, ModuleWrapper>();
		private readonly Lifetime _lifetime;
		private readonly IShellLocks _shellLocks;
		private readonly ChangeManager _changeManager;
		private readonly T4Environment _t4Environment;

		private struct ModuleWrapper {
			internal readonly T4PsiModule Module;
			internal readonly LifetimeDefinition LifetimeDefinition;

			internal ModuleWrapper([NotNull] T4PsiModule module, [NotNull] LifetimeDefinition lifetimeDefinition) {
				Module = module;
				LifetimeDefinition = lifetimeDefinition;
			}
		}

		/// <summary>
		/// Gets all <see cref="T4PsiModule"/>s for opened files.
		/// </summary>
		/// <returns>A collection of <see cref="T4PsiModule"/>.</returns>
		internal IEnumerable<IPsiModule> GetModules() {
			_shellLocks.AssertReadAccessAllowed();

			return _modules.Values.Select(wrapper => (IPsiModule) wrapper.Module);
		}

		/// <summary>
		/// Gets all source files for a given project file.
		/// </summary>
		/// <param name="projectFile">The project file whose source files will be returned.</param>
		[NotNull]
		internal IList<IPsiSourceFile> GetPsiSourceFilesFor(IProjectFile projectFile) {
			_shellLocks.AssertReadAccessAllowed();

			ModuleWrapper wrapper;
			return projectFile != null && projectFile.IsValid() && _modules.TryGetValue(projectFile, out wrapper) && wrapper.Module.IsValid()
				? new[] { wrapper.Module.SourceFile }
			    : EmptyList<IPsiSourceFile>.InstanceList;
		}

		/// <summary>
		/// Processes changes for specific project file and returns a list of corresponding source file changes.
		/// </summary>
		/// <param name="projectFile">The project file.</param>
		/// <param name="changeType">Type of the change.</param>
		/// <param name="changeBuilder">The change builder used to populate changes.</param>
		internal void OnProjectFileChanged(IProjectFile projectFile, PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder) {
			if (!_t4Environment.IsSupported)
				return;

			_shellLocks.AssertWriteAccessAllowed();
			ModuleWrapper moduleWrapper;

			switch (changeType) {

				case PsiModuleChange.ChangeType.ADDED:
					if (projectFile.LanguageType.Is<T4ProjectFileType>())
						AddFile(projectFile, changeBuilder);
					break;

				case PsiModuleChange.ChangeType.REMOVED:
					if (_modules.TryGetValue(projectFile, out moduleWrapper))
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
					break;

				case PsiModuleChange.ChangeType.MODIFIED:
					if (_modules.TryGetValue(projectFile, out moduleWrapper))
						ModifyFile(changeBuilder, moduleWrapper);
					break;

			}
		}

		private void AddFile([NotNull] IProjectFile projectFile, [NotNull] PsiModuleChangeBuilder changeBuilder) {
			ISolution solution = projectFile.GetSolution();

			// creates a new T4PsiModule for the file
			LifetimeDefinition lifetimeDefinition = Lifetimes.Define(_lifetime, "[T4]" + projectFile.Name);
			var psiModule = new T4PsiModule(
				lifetimeDefinition.Lifetime,
				solution.GetComponent<PsiModuleManager>(),
				solution.GetComponent<DocumentManager>(),
				_changeManager,
				solution.GetComponent<IAssemblyFactory>(),
				_shellLocks,
				projectFile,
				solution.GetComponent<T4FileDataCache>(),
				_t4Environment);
			_modules[projectFile] = new ModuleWrapper(psiModule, lifetimeDefinition);
			changeBuilder.AddModuleChange(psiModule, PsiModuleChange.ChangeType.ADDED);
			changeBuilder.AddFileChange(psiModule.SourceFile, PsiModuleChange.ChangeType.ADDED);

			// Invalidate files that had this specific files as an include,
			// and whose IPsiSourceFile was previously managed by T4OutsideSolutionSourceFileManager.
			// Those files will be reparsed with the new source file.
			var fileManager = solution.GetComponent<T4OutsideSolutionSourceFileManager>();
			FileSystemPath location = projectFile.Location;
			if (fileManager.HasSourceFile(location)) {
				fileManager.DeleteSourceFile(location);
				InvalidateFilesHavingInclude(location, solution.GetPsiServices());
			}
		}

		private void RemoveFile([NotNull] IProjectFile projectFile, [NotNull] PsiModuleChangeBuilder changeBuilder, ModuleWrapper moduleWrapper) {
			_modules.Remove(projectFile);
			changeBuilder.AddFileChange(moduleWrapper.Module.SourceFile, PsiModuleChange.ChangeType.REMOVED);
			changeBuilder.AddModuleChange(moduleWrapper.Module, PsiModuleChange.ChangeType.REMOVED);
			InvalidateFilesHavingInclude(projectFile.Location, moduleWrapper.Module.GetPsiServices());
			moduleWrapper.LifetimeDefinition.Terminate();
		}

		private static void ModifyFile([NotNull] PsiModuleChangeBuilder changeBuilder, ModuleWrapper moduleWrapper) {
			changeBuilder.AddFileChange(moduleWrapper.Module.SourceFile, PsiModuleChange.ChangeType.MODIFIED);
		}

		private void InvalidateFilesHavingInclude([NotNull] FileSystemPath includeLocation, [NotNull] IPsiServices psiServices) {
			foreach (ModuleWrapper moduleWrapper in _modules.Values) {
				IPsiSourceFile sourceFile = moduleWrapper.Module.SourceFile;
				var t4File = sourceFile.GetTheOnlyPsiFile(T4Language.Instance) as IT4File;
				if (t4File != null && t4File.GetNonEmptyIncludePaths().Any(path => path == includeLocation))
					psiServices.MarkAsDirty(sourceFile);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			using (WriteLockCookie.Create()) {
				foreach (var wrapper in _modules.Values)
					wrapper.LifetimeDefinition.Terminate();
				_modules.Clear();
			}
		}

		public T4PsiModuleProvider([NotNull] Lifetime lifetime, [NotNull] IShellLocks shellLocks, [NotNull] ChangeManager changeManager, [NotNull] T4Environment t4Environment) {
			_lifetime = lifetime;
			_shellLocks = shellLocks;
			_changeManager = changeManager;
			_t4Environment = t4Environment;
		}

	}

}