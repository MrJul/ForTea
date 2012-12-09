using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Web.Impl.PsiModules;
using JetBrains.Threading;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// PSI module managing a single T4 file.
	/// </summary>
	internal sealed class T4PsiModule : IProjectPsiModule, IChangeProvider {
		private const string Prefix = "[T4] ";
		
		private readonly Dictionary<string, IAssemblyCookie> _assemblyReferences = new Dictionary<string, IAssemblyCookie>(StringComparer.OrdinalIgnoreCase);
		private readonly Lifetime _lifetime;
		private readonly PsiModuleManager _psiModuleManager;
		private readonly DocumentManager _documentManager;
		private readonly ChangeManager _changeManager;
		private readonly IAssemblyFactory _assemblyFactory;
		private readonly IShellLocks _shellLocks;
		private readonly IProject _project;
		private readonly ISolution _solution;
		private readonly IProjectFile _projectFile;
		private readonly T4Environment _t4Environment;
		private readonly IPsiSourceFile _sourceFile;
		private readonly T4ResolveProject _resolveProject;
		private IModuleReferenceResolveManager _resolveManager;
		private bool _isValid;
		
		/// <summary>
		/// Returns the source file associated with this PSI module.
		/// </summary>
		[NotNull]
		internal IPsiSourceFile SourceFile {
			get { return _sourceFile; }
		}

		/// <summary>
		/// Gets an instance of <see cref="IModuleReferenceResolveManager"/> used to resolve assemblies.
		/// </summary>
		[NotNull]
		private IModuleReferenceResolveManager ResolveManager {
			get { return _resolveManager ?? (_resolveManager = _solution.GetComponent<IModuleReferenceResolveManager>()); }
		}

		/// <summary>
		/// Gets the name of this PSI module.
		/// </summary>
		public string Name {
			get { return Prefix + _sourceFile.Name; }
		}

		/// <summary>
		/// Gets the display name of this PSI module.
		/// </summary>
		public string DisplayName {
			get { return Prefix + _sourceFile.DisplayName; }
		}

		/// <summary>
		/// Gets the language used by this PSI module: always <see cref="T4Language"/>.
		/// </summary>
		public PsiLanguageType PsiLanguage {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the project file type used by this PSI module: always <see cref="JetBrains.ProjectModel.ProjectFileType"/>.
		/// </summary>
		public ProjectFileType ProjectFileType {
			get { return T4ProjectFileType.Instance; }
		}
		
		IModule IPsiModule.ContainingProjectModule {
			get { return null; }
		}

		IEnumerable<IPsiSourceFile> IPsiModule.SourceFiles {
			get { return new[] { _sourceFile }; }
		}

		IProject IProjectPsiModule.Project {
			get { return _project; }
		}

		/// <summary>
		/// Gets the solution this PSI module is attached to.
		/// </summary>
		/// <returns>An instance of <see cref="ISolution"/>.</returns>
		[NotNull]
		public ISolution GetSolution() {
			return _solution;
		}

		/// <summary>
		/// Gets an instance of <see cref="IPsiServices"/> for the current solution.
		/// </summary>
		/// <returns>An instance of <see cref="IPsiServices"/>.</returns>
		public IPsiServices GetPsiServices() {
			return _solution.GetPsiServices();
		}

		/// <summary>
		/// Gets whether the PSI module is valid.
		/// </summary>
		/// <returns><c>true</c> if the PSI module is valid, <c>false</c> otherwise.</returns>
		public bool IsValid() {
			return _isValid;
		}

		/// <summary>
		/// Gets a persistent identifier for this PSI module.
		/// </summary>
		/// <returns>A persistent identifier.</returns>
		public string GetPersistentID() {
			return Prefix + _sourceFile.GetPersistentID();
		}

		IList<PreProcessingDirective> IPsiModule.GetAllDefines() {
			return EmptyList<PreProcessingDirective>.InstanceList;
		}

		/// <summary>
		/// Creates a new <see cref="IAssemblyCookie"/> from an assembly full name.
		/// </summary>
		/// <param name="assemblyFullName">The assembly full name.</param>
		/// <returns>An instance of <see cref="IAssemblyCookie"/>, or <c>null</c> if none could be created.</returns>
		[CanBeNull]
		private IAssemblyCookie CreateCookie(string assemblyFullName) {
			if (assemblyFullName == null || (assemblyFullName = assemblyFullName.Trim()).Length == 0)
				return null;

			var nameInfo = AssemblyNameInfo.TryParse(assemblyFullName);
			if (nameInfo == null)
				return null;

			var target = new AssemblyReferenceTarget(nameInfo, FileSystemPath.Empty);
			AssemblyReferenceResolveResult result = ResolveManager.Resolve(target, _resolveProject);
			if (result == null)
				return null;

			return _assemblyFactory.AddRef(result, "T4", _t4Environment.PlatformID);
		}

		/// <summary>
		/// Try to add an assembly reference to the list of assemblies.
		/// </summary>
		/// <param name="assemblyFullName"></param>
		/// <remarks>Does not refresh references, simply add a cookie to the cookies list.</remarks>
		[CanBeNull]
		private IAssemblyCookie TryAddReference([NotNull] string assemblyFullName) {
			var cookie = CreateCookie(assemblyFullName);
			if (cookie != null)
				_assemblyReferences.Add(assemblyFullName, cookie);
			return cookie;
		}

		object IChangeProvider.Execute(IChangeMap changeMap) {
			return null;
		}

		private void OnDataFileChanged(Pair<IPsiSourceFile, T4FileDataDiff> pair) {
			if (pair.First != _sourceFile)
				return;

			if (_shellLocks.IsWriteAccessAllowed)
				OnDataFileChanged(pair.Second);
			else {
				_shellLocks.ExecuteOrQueue(_lifetime, "T4PsiModuleOnFileDataChanged",
					() => _shellLocks.ExecuteWithWriteLock(() => OnDataFileChanged(pair.Second)));
			}
		}

		/// <summary>
		/// Called when the associated data file changed: added/removed assemblies or includes.
		/// </summary>
		/// <param name="dataDiff">The difference between the old and new data.</param>
		private void OnDataFileChanged([NotNull] T4FileDataDiff dataDiff) {
			_shellLocks.AssertWriteAccessAllowed();

			bool hasChanges = false;

			foreach (string removedAssembly in dataDiff.RemovedAssemblies) {
				IAssemblyCookie cookie;
				if (!_assemblyReferences.TryGetValue(removedAssembly, out cookie))
					continue;
				_assemblyReferences.Remove(removedAssembly);
				hasChanges = true;
				cookie.Dispose();
			}

			foreach (string addedAssembly in dataDiff.AddedAssemblies) {
				if (_assemblyReferences.ContainsKey(addedAssembly))
					continue;
				IAssemblyCookie cookie = TryAddReference(addedAssembly);
				if (cookie != null)
					hasChanges = true;
			}

			if (!hasChanges)
				return;

			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.MODIFIED);
			_shellLocks.ExecuteOrQueue("T4PsiModuleChange",
				() => _changeManager.ExecuteAfterChange(
					() => _shellLocks.ExecuteWithWriteLock(
						() => _changeManager.OnProviderChanged(this, changeBuilder.Result, SimpleTaskExecutor.Instance)
					)
				)
			);
		}

		/// <summary>
		/// Gets all modules referenced by this module.
		/// </summary>
		/// <returns>All referenced modules.</returns>
		public IEnumerable<IPsiModuleReference> GetReferences() {
			_shellLocks.AssertReadAccessAllowed();
			
			var references = new PsiModuleReferenceAccumulator();
			foreach (IAssemblyCookie cookie in _assemblyReferences.Values) {
				if (cookie.Assembly == null)
					continue;
				IPsiModule psiModule = _psiModuleManager.GetPrimaryPsiModule(cookie.Assembly);
				if (psiModule != null)
					references.Add(new PsiModuleReference(psiModule));
			}
			return references.GetReferences();
		}

		/// <summary>
		/// Disposes this instance.
		/// </summary>
		/// <remarks>Does not implement <see cref="IDisposable"/>, is called when the lifetime is terminated.</remarks>
		private void Dispose() {
			_isValid = false;
			
			foreach (IAssemblyCookie assemblyCookie in _assemblyReferences.Values)
				assemblyCookie.Dispose();
			_assemblyReferences.Clear();
			_resolveProject.Dispose();
		}

		private void AddBaseReferences() {
			TryAddReference("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			TryAddReference("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			foreach (string assemblyName in _t4Environment.TextTemplatingAssemblyNames)
				TryAddReference(assemblyName);
		}

		internal T4PsiModule([NotNull] Lifetime lifetime, [NotNull] PsiModuleManager psiModuleManager, [NotNull] DocumentManager documentManager,
			[NotNull] ChangeManager changeManager, [NotNull] IAssemblyFactory assemblyFactory, [NotNull] IShellLocks shellLocks,
			[NotNull] IProjectFile projectFile, [NotNull] T4FileDataCache fileDataCache, T4Environment t4Environment) {

			_lifetime = lifetime;
			lifetime.AddAction(Dispose);
			
			_psiModuleManager = psiModuleManager;
			_documentManager = documentManager;
			_assemblyFactory = assemblyFactory;

			_changeManager = changeManager;
			changeManager.RegisterChangeProvider(lifetime, this);
			changeManager.AddDependency(lifetime, psiModuleManager, this);
			
			_shellLocks = shellLocks;
			_projectFile = projectFile;
			_project = projectFile.GetProject();
			Assertion.AssertNotNull(_project, "_project != null");
			_solution = _project.GetSolution();
			
			_t4Environment = t4Environment;
			_resolveProject = new T4ResolveProject(_solution, _shellLocks, t4Environment.PlatformID, _project);

			_sourceFile = new PsiProjectFile(
				this,
				_projectFile,
				(pf, sf) => new DefaultPsiProjectFileProperties(pf, sf),
				JetFunc<IProjectFile, IPsiSourceFile>.True,
				_documentManager);

			_isValid = true;
			fileDataCache.FileDataChanged.Advise(lifetime, OnDataFileChanged);
			AddBaseReferences();
		}

	}

}