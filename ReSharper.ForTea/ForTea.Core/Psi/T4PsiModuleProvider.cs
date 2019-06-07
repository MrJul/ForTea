using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {

	/// <summary>
	/// Manages <see cref="IT4PsiModule"/> for T4 files.
	/// Contains common implementation for <see cref="T4ProjectPsiModuleHandler"/> and <see cref="T4MiscFilesProjectPsiModuleProvider"/>.
	/// </summary>
	internal sealed class T4PsiModuleProvider : IDisposable {

		[NotNull] private readonly Dictionary<IProjectFile, ModuleWrapper> _modules = new Dictionary<IProjectFile, ModuleWrapper>();
		private readonly Lifetime _lifetime;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly IT4MacroResolver _resolver;

		private readonly struct ModuleWrapper {

			[NotNull] public readonly IT4PsiModule Module;
			[NotNull] public readonly LifetimeDefinition LifetimeDefinition;

			public ModuleWrapper([NotNull] IT4PsiModule module, [NotNull] LifetimeDefinition lifetimeDefinition) {
				Module = module;
				LifetimeDefinition = lifetimeDefinition;
			}
		}

		/// <summary>Gets all <see cref="IT4PsiModule"/>s for opened files.</summary>
		/// <returns>A collection of <see cref="IT4PsiModule"/>.</returns>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<IPsiModule> GetModules() {
			_shellLocks.AssertReadAccessAllowed();

			return _modules.Values.Select(wrapper => (IPsiModule) wrapper.Module);
		}

		/// <summary>Gets all source files for a given project file.</summary>
		/// <param name="projectFile">The project file whose source files will be returned.</param>
		[NotNull]
		[ItemNotNull]
		public IList<IPsiSourceFile> GetPsiSourceFilesFor([CanBeNull] IProjectFile projectFile) {
			_shellLocks.AssertReadAccessAllowed();

			return projectFile != null
				&& projectFile.IsValid()
				&& _modules.TryGetValue(projectFile, out ModuleWrapper wrapper)
				&& wrapper.Module.IsValid()
			? new[] { wrapper.Module.SourceFile }
			: EmptyList<IPsiSourceFile>.InstanceList;
		}

		/// <summary>Processes changes for specific project file and returns a list of corresponding source file changes.</summary>
		/// <param name="projectFile">The project file.</param>
		/// <param name="changeType">Type of the change.</param>
		/// <param name="changeBuilder">The change builder used to populate changes.</param>
		/// <returns>Whether the provider has handled the file change.</returns>
		public bool OnProjectFileChanged(
			[NotNull] IProjectFile projectFile,
			ref PsiModuleChange.ChangeType changeType,
			[NotNull] PsiModuleChangeBuilder changeBuilder
		) {
			if (!_t4Environment.IsSupported)
				return false;

			_shellLocks.AssertWriteAccessAllowed();
			ModuleWrapper moduleWrapper;
			
			switch (changeType) {

				case PsiModuleChange.ChangeType.Added:
					// Preprocessed .tt files should be handled by R# itself as if it's a normal project file,
					// so that it has access to the current project types.
					if (projectFile.LanguageType.Is<T4ProjectFileType>() && !projectFile.IsPreprocessedT4Template()) {
						AddFile(projectFile, changeBuilder);
						return true;
					}
					break;

				case PsiModuleChange.ChangeType.Removed:
					if (_modules.TryGetValue(projectFile, out moduleWrapper)) {
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
						return true;
					}
					break;

				case PsiModuleChange.ChangeType.Modified:
					if (_modules.TryGetValue(projectFile, out moduleWrapper)) {
						if (!projectFile.IsPreprocessedT4Template()) {
							ModifyFile(changeBuilder, moduleWrapper);
							return true;
						}

						// The T4 file went from Transformed to Preprocessed, it doesn't need a T4PsiModule anymore.
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
						changeType = PsiModuleChange.ChangeType.Added;
						return false;
					}

					// The T4 file went from Preprocessed to Transformed, it now needs a T4PsiModule.
					if (projectFile.LanguageType.Is<T4ProjectFileType>() && !projectFile.IsPreprocessedT4Template()) {
						AddFile(projectFile, changeBuilder);
						changeType = PsiModuleChange.ChangeType.Removed;
						return false;
					}

					break;

			}

			return false;
		}

		private void AddFile([NotNull] IProjectFile projectFile, [NotNull] PsiModuleChangeBuilder changeBuilder) {
			ISolution solution = projectFile.GetSolution();

			// creates a new T4PsiModule for the file
			LifetimeDefinition lifetimeDefinition = Lifetime.Define(_lifetime, "[T4]" + projectFile.Name);
			IT4PsiModule psiModule = new T4PsiModule(
				lifetimeDefinition.Lifetime,
				solution.GetComponent<IPsiModules>(),
				solution.GetComponent<DocumentManager>(),
				_changeManager,
				solution.GetComponent<IAssemblyFactory>(),
				_shellLocks,
				T4TemplateInfo.FromFile(projectFile),
				solution.GetComponent<T4FileDataCache>(),
				_t4Environment,
				solution.GetComponent<OutputAssemblies>(),
				_resolver
			);
			_modules[projectFile] = new ModuleWrapper(psiModule, lifetimeDefinition);
			changeBuilder.AddModuleChange(psiModule, PsiModuleChange.ChangeType.Added);
			changeBuilder.AddFileChange(psiModule.SourceFile, PsiModuleChange.ChangeType.Added);

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
			changeBuilder.AddFileChange(moduleWrapper.Module.SourceFile, PsiModuleChange.ChangeType.Removed);
			changeBuilder.AddModuleChange(moduleWrapper.Module, PsiModuleChange.ChangeType.Removed);
			InvalidateFilesHavingInclude(projectFile.Location, moduleWrapper.Module.GetPsiServices());
			moduleWrapper.LifetimeDefinition.Terminate();
		}

		private static void ModifyFile([NotNull] PsiModuleChangeBuilder changeBuilder, ModuleWrapper moduleWrapper)
			=> changeBuilder.AddFileChange(moduleWrapper.Module.SourceFile, PsiModuleChange.ChangeType.Modified);

		private void InvalidateFilesHavingInclude([NotNull] FileSystemPath includeLocation, [NotNull] IPsiServices psiServices) {
			psiServices.GetComponent<T4FileDependencyManager>().UpdateIncludes(includeLocation, EmptyList<FileSystemPath>.InstanceList);
			foreach (ModuleWrapper moduleWrapper in _modules.Values) {
				IPsiSourceFile sourceFile = moduleWrapper.Module.SourceFile;
				if (sourceFile.GetTheOnlyPsiFile(T4Language.Instance) is IT4File t4File 
				&& t4File.GetNonEmptyIncludePaths().Any(path => path == includeLocation))
					psiServices.MarkAsDirty(sourceFile);
			}
		}
		
		public void Dispose() {
			using (WriteLockCookie.Create()) {
				foreach (var wrapper in _modules.Values)
					wrapper.LifetimeDefinition.Terminate();
				_modules.Clear();
			}
		}

		internal T4PsiModuleProvider(
			Lifetime lifetime,
			[NotNull] IShellLocks shellLocks,
			[NotNull] ChangeManager changeManager,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IT4MacroResolver resolver
		)
		{
			_lifetime = lifetime;
			_shellLocks = shellLocks;
			_changeManager = changeManager;
			_t4Environment = t4Environment;
			_resolver = resolver;
		}

	}

}