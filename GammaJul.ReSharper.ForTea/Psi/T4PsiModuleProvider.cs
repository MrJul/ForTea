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
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Transaction;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	/// <summary>
	/// Manages <see cref="T4PsiModule"/> for T4 files.
	/// Contains common implementation for <see cref="T4ProjectPsiModuleHandler"/> and <see cref="T4MiscFilesProjectPsiModuleProvider"/>.
	/// </summary>
	[SolutionComponent]
	public sealed class T4PsiModuleProvider : IDisposable, IChangeProvider {
		private readonly Dictionary<IProjectFile, ModuleWrapper> _modules = new Dictionary<IProjectFile, ModuleWrapper>();
		private readonly Lifetime _lifetime;
		private readonly IShellLocks _shellLocks;
		private readonly ChangeManager _changeManager;
		private readonly T4Environment _t4Environment;
		private readonly ISolution _solution;

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
		/// <returns>Whether the provider has handled the file change.</returns>
		internal bool OnProjectFileChanged(IProjectFile projectFile, ref PsiModuleChange.ChangeType changeType, PsiModuleChangeBuilder changeBuilder) {
			if (!_t4Environment.IsSupported || !projectFile.LanguageType.Is<T4ProjectFileType>())
				return false;

			_shellLocks.AssertWriteAccessAllowed();
			ModuleWrapper moduleWrapper;
			
			switch (changeType) {

				case PsiModuleChange.ChangeType.ADDED:
					// Preprocessed .tt files should be handled by R# itself as if it's a normal project file,
					// so that it has access to the current project types.
					if (!projectFile.IsPreprocessedT4Template()) {
						AddFile(projectFile, changeBuilder);
						return true;
					}
					break;

				case PsiModuleChange.ChangeType.REMOVED:
					if (_modules.TryGetValue(projectFile, out moduleWrapper)) {
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
						return true;
					}
					break;

				case PsiModuleChange.ChangeType.MODIFIED:
					if (_modules.TryGetValue(projectFile, out moduleWrapper)) {
						if (!projectFile.IsPreprocessedT4Template()) {
							ModifyFile(changeBuilder, moduleWrapper);
							return true;
						}

						// The T4 file went from Transformed to Preprocessed, it doesn't need a T4PsiModule anymore.
						RemoveFile(projectFile, changeBuilder, moduleWrapper);
						changeType = PsiModuleChange.ChangeType.ADDED;
						return false;
					}

					// The T4 file went from Preprocessed to Transformed, it now needs a T4PsiModule.
					if (!projectFile.IsPreprocessedT4Template()) {
						AddFile(projectFile, changeBuilder);
						changeType = PsiModuleChange.ChangeType.REMOVED;
						return false;
					}

					break;

			}

			return false;
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

		public object Execute(IChangeMap changeMap) {
			var change = changeMap.GetChange<ProjectModelChange>(_solution);
			if (change != null && _solution.IsValid())
				change.Accept(new Abc());

			return null;
		}

		private class Abc : RecursiveProjectModelChangeDeltaVisitor {

			public override void VisitItemDelta(ProjectItemChange change) {
				base.VisitItemDelta(change);
				var projectFile = change.ProjectItem as IProjectFile;
				if (projectFile != null && change.ContainsChangeType(ProjectModelChangeType.PROPERTIES)) {
				}
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

		public T4PsiModuleProvider([NotNull] Lifetime lifetime, [NotNull] IShellLocks shellLocks, [NotNull] ChangeManager changeManager,
			[NotNull] T4Environment t4Environment, [NotNull] ISolution solution) {
			_lifetime = lifetime;
			_shellLocks = shellLocks;
			_changeManager = changeManager;
			_t4Environment = t4Environment;
			_solution = solution;

			changeManager.RegisterChangeProvider(lifetime, this);
			changeManager.AddDependency(lifetime, this, solution);
		}

	}

}