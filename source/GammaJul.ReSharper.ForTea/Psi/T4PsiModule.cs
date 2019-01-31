using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.Interop.WinApi;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Web.Impl.PsiModules;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.Interop.Shim.VsShell.Shell.Hierarchy;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>PSI module managing a single T4 file.</summary>
	internal sealed class T4PsiModule : IProjectPsiModule, IChangeProvider {

		private const string Prefix = "[T4] ";
		
		[NotNull] private readonly Dictionary<string, IAssemblyCookie> _assemblyReferences = new Dictionary<string, IAssemblyCookie>(StringComparer.OrdinalIgnoreCase);
		[NotNull] private readonly Dictionary<string, string> _resolvedMacros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		[NotNull] private readonly Lifetime _lifetime;
		[NotNull] private readonly IPsiModules _psiModules;
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IAssemblyFactory _assemblyFactory;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly IProjectFile _projectFile;
		[NotNull] private readonly IProject _project;
		[NotNull] private readonly ISolution _solution;
		[NotNull] private readonly T4Environment _t4Environment;
		[NotNull] private readonly T4ResolveProject _resolveProject;
		[NotNull] private readonly OutputAssemblies _outputAssemblies;
		[NotNull] private readonly IModuleReferenceResolveContext _moduleReferenceResolveContext;
		[NotNull] private readonly UserDataHolder _userDataHolder = new UserDataHolder();

		[CanBeNull] private IModuleReferenceResolveManager _resolveManager;
		private bool _isValid;

		/// <summary>Returns the source file associated with this PSI module.</summary>
		[NotNull]
		public IPsiSourceFile SourceFile { get; }
		
		/// <summary>Gets an instance of <see cref="IModuleReferenceResolveManager"/> sed to resolve assemblies.</summary>
		[NotNull]
		private IModuleReferenceResolveManager ResolveManager
			=> _resolveManager ?? (_resolveManager = _solution.GetComponent<IModuleReferenceResolveManager>());

		/// <summary>Gets the name of this PSI module.</summary>
		public string Name
			=> Prefix + SourceFile.Name;

		/// <summary>Gets the display name of this PSI module.</summary>
		public string DisplayName
			=> Prefix + SourceFile.DisplayName;

		/// <summary>Gets the language used by this PSI module. This should be the code behind language, not the primary language.</summary>
		public PsiLanguageType PsiLanguage
			=> SourceFile.GetLanguages().FirstOrDefault(lang => !lang.Is<T4Language>()) ?? UnknownLanguage.Instance;

		/// <summary>Gets the project file type used by this PSI module: always <see cref="JetBrains.ProjectModel.ProjectFileType"/>.</summary>
		public ProjectFileType ProjectFileType
			=> T4ProjectFileType.Instance;

		IModule IPsiModule.ContainingProjectModule
			=> null;

		IEnumerable<IPsiSourceFile> IPsiModule.SourceFiles
			=> new[] { SourceFile };

		IProject IProjectPsiModule.Project
			=> _project;

		/// <summary>TargetFrameworkId corresponding to the module.</summary>
		public TargetFrameworkId TargetFrameworkId
			=> _t4Environment.TargetFrameworkId;

		/// <summary>Gets the solution this PSI module is attached to.</summary>
		/// <returns>An instance of <see cref="ISolution"/>.</returns>
		public ISolution GetSolution()
			=> _solution;

		/// <summary>Gets an instance of <see cref="IPsiServices"/> for the current solution.</summary>
		/// <returns>An instance of <see cref="IPsiServices"/>.</returns>
		public IPsiServices GetPsiServices()
			=> _solution.GetPsiServices();

		/// <summary>Gets whether the PSI module is valid.</summary>
		/// <returns><c>true</c> if the PSI module is valid, <c>false</c> otherwise.</returns>
		public bool IsValid()
			=> _isValid;

		/// <summary>Gets a persistent identifier for this PSI module.</summary>
		/// <returns>A persistent identifier.</returns>
		public string GetPersistentID()
			=> Prefix + _projectFile.GetPersistentID();

		/// <summary>Creates a new <see cref="IAssemblyCookie"/> from an assembly full name.</summary>
		/// <param name="assemblyNameOrFile">The assembly full name.</param>
		/// <returns>An instance of <see cref="IAssemblyCookie"/>, or <c>null</c> if none could be created.</returns>
		[CanBeNull]
		private IAssemblyCookie CreateCookie(string assemblyNameOrFile) {
			if (assemblyNameOrFile == null || (assemblyNameOrFile = assemblyNameOrFile.Trim()).Length == 0)
				return null;

			AssemblyReferenceTarget target = null;

			// assembly path
			FileSystemPath path = FileSystemPath.TryParse(assemblyNameOrFile);
			if (!path.IsEmpty && path.IsAbsolute)
				target = new AssemblyReferenceTarget(AssemblyNameInfo.Empty, path);
			
			// assembly name
			else {
				AssemblyNameInfo nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
				if (nameInfo != null)
					target = new AssemblyReferenceTarget(nameInfo, FileSystemPath.Empty);
			}

			if (target == null)
				return null;
			
			return CreateCookieCore(target);
		}
		
		/// <summary>Try to add an assembly reference to the list of assemblies.</summary>
		/// <param name="assemblyNameOrFile"></param>
		/// <remarks>Does not refresh references, simply add a cookie to the cookies list.</remarks>
		[CanBeNull]
		private IAssemblyCookie TryAddReference([NotNull] string assemblyNameOrFile) {
			var cookie = CreateCookie(assemblyNameOrFile);
			if (cookie != null)
				_assemblyReferences.Add(assemblyNameOrFile, cookie);
			return cookie;
		}
		
		/// <summary>The <see cref="IVsHierarchy"/> representing the project file normally implements <see cref="IVsBuildMacroInfo"/>.</summary>
		/// <returns>An instance of <see cref="IVsBuildMacroInfo"/> if found.</returns>
		[CanBeNull]
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private IVsBuildMacroInfo TryGetVsBuildMacroInfo()
			=> TryGetVsHierarchy() as IVsBuildMacroInfo;

		[CanBeNull]
		private IVsHierarchy TryGetVsHierarchy() {
			var synchronizer = _solution.TryGetComponent<ProjectModelSynchronizer>();
			if (synchronizer == null)
				return null;

			VsHierarchyItem hierarchyItem = synchronizer.TryGetHierarchyItemByProjectItem(_projectFile, false);
			if (hierarchyItem == null)
				return null;

			return hierarchyItem.Hierarchy;
		}

		private void OnDataFileChanged(Pair<IPsiSourceFile, T4FileDataDiff> pair) {
			if (pair.First != SourceFile)
				return;

			if (_shellLocks.IsWriteAccessAllowed())
				OnDataFileChanged(pair.Second);
			else {
				_shellLocks.ExecuteOrQueue(_lifetime, "T4PsiModuleOnFileDataChanged",
					() => _shellLocks.ExecuteWithWriteLock(() => OnDataFileChanged(pair.Second)));
			}
		}

		/// <summary>Called when the associated data file changed: added/removed assemblies or includes.</summary>
		/// <param name="dataDiff">The difference between the old and new data.</param>
		private void OnDataFileChanged([NotNull] T4FileDataDiff dataDiff) {
			_shellLocks.AssertWriteAccessAllowed();

			bool hasFileChanges = ResolveMacros(dataDiff.AddedMacros);
			bool hasChanges = hasFileChanges;
			
			ITextTemplatingComponents components = _t4Environment.Components.CanBeNull;
			using (components.With(TryGetVsHierarchy(), _projectFile.Location)) {
				
				// removes the assembly references from the old assembly directives
				foreach (string removedAssembly in dataDiff.RemovedAssemblies) {
					string assembly = removedAssembly;
					if (components != null)
						assembly = components.Host.ResolveAssemblyReference(assembly);

					if (!_assemblyReferences.TryGetValue(assembly, out IAssemblyCookie cookie))
						continue;

					_assemblyReferences.Remove(assembly);
					hasChanges = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives
				foreach (string addedAssembly in dataDiff.AddedAssemblies) {
					string assembly = addedAssembly;
					if (components != null)
						assembly = components.Host.ResolveAssemblyReference(assembly);

					if (assembly == null)
						continue;

					if (_assemblyReferences.ContainsKey(assembly))
						continue;

					IAssemblyCookie cookie = TryAddReference(assembly);
					if (cookie != null)
						hasChanges = true;
				}

			}
			
			if (!hasChanges)
				return;

			// tells the world the module has changed
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.Modified);

			if (hasFileChanges)
				GetPsiServices().MarkAsDirty(SourceFile);

			_shellLocks.ExecuteOrQueue("T4PsiModuleChange",
				() => _changeManager.ExecuteAfterChange(
					() => _shellLocks.ExecuteWithWriteLock(
						() => _changeManager.OnProviderChanged(this, changeBuilder.Result, SimpleTaskExecutor.Instance)
					)
				)
			);
		}
		
		/// <summary>Resolves new VS macros, like $(SolutionDir), found in include or assembly directives.</summary>
		/// <param name="macros">The list of macro names (eg SolutionDir) to resolve.</param>
		/// <returns>Whether at least one macro has been processed.</returns>
		private bool ResolveMacros([NotNull] IEnumerable<string> macros) {
			bool hasChanges = false;

			IVsBuildMacroInfo vsBuildMacroInfo = null;
			foreach (string addedMacro in macros) {
				if (vsBuildMacroInfo == null) {
					vsBuildMacroInfo = TryGetVsBuildMacroInfo();
					if (vsBuildMacroInfo == null) {
						Logger.LogError("Couldn't get IVsBuildMacroInfo");
						break;
					}
				}

				hasChanges = true;

				bool succeeded = HResultHelpers.SUCCEEDED(vsBuildMacroInfo.GetBuildMacroValue(addedMacro, out string value)) && !String.IsNullOrEmpty(value);
				if (!succeeded) {
					value = MSBuildExtensions.GetStringValue(TryGetVsHierarchy(), addedMacro, null);
					succeeded = !String.IsNullOrEmpty(value);
				}
				
				lock (_resolvedMacros) {
					if (succeeded)
						_resolvedMacros[addedMacro] = value;
					else
						_resolvedMacros.Remove(addedMacro);
				}
			}

			return hasChanges;
		}

		/// <summary>Gets all modules referenced by this module.</summary>
		/// <returns>All referenced modules.</returns>
		public IEnumerable<IPsiModuleReference> GetReferences(IModuleReferenceResolveContext moduleReferenceResolveContext) {
			_shellLocks.AssertReadAccessAllowed();
			
			var references = new PsiModuleReferenceAccumulator(TargetFrameworkId);
			
			foreach (IAssemblyCookie cookie in _assemblyReferences.Values) {
				if (cookie.Assembly == null)
					continue;

				IPsiModule psiModule = _psiModules.GetPrimaryPsiModule(cookie.Assembly, TargetFrameworkId);

				// Normal assembly.
				if (psiModule != null)
					references.Add(new PsiModuleReference(psiModule));

				// Assembly that is the output of a current project: reference the project instead.
				else {
					IProject project = _outputAssemblies.GetProjectByOutputAssembly(cookie.Assembly);
					if (project != null) {
						psiModule = _psiModules.GetPrimaryPsiModule(project, TargetFrameworkId);
						if (psiModule != null)
							references.Add(new PsiModuleReference(psiModule));
					}
				}
			}

			return references.GetReferences();
		}

		[NotNull]
		public IDictionary<string, string> GetResolvedMacros() {
			lock (_resolvedMacros) {
				if (_resolvedMacros.Count > 0)
					return new Dictionary<string, string>(_resolvedMacros);
				return EmptyDictionary<string, string>.Instance;
			}
		}

		object IChangeProvider.Execute(IChangeMap changeMap)
			=> null;

		ICollection<PreProcessingDirective> IPsiModule.GetAllDefines()
			=> EmptyList<PreProcessingDirective>.InstanceList;

		[NotNull]
		private PsiProjectFile CreateSourceFile([NotNull] IProjectFile projectFile, [NotNull] DocumentManager documentManager)
			=> new PsiProjectFile(
				this,
				projectFile,
				(pf, sf) => new T4PsiProjectFileProperties(pf, sf, true),
				JetFunc<IProjectFile, IPsiSourceFile>.True,
				documentManager,
				_moduleReferenceResolveContext
			);

		[CanBeNull]
		private IAssemblyCookie CreateCookieCore([NotNull] AssemblyReferenceTarget target) {
			FileSystemPath result = ResolveManager.Resolve(target, _resolveProject, _moduleReferenceResolveContext);
			return result != null
				? _assemblyFactory.AddRef(result, "T4", _moduleReferenceResolveContext)
				: null;
		}


		public T GetData<T>(Key<T> key)
		where T : class
			=> _userDataHolder.GetData(key);

		public void PutData<T>(Key<T> key, T val)
		where T : class
			=> _userDataHolder.PutData(key, val);

		public T GetOrCreateDataUnderLock<T>(Key<T> key, Func<T> factory)
		where T : class
			=> _userDataHolder.GetOrCreateDataUnderLock(key, factory);

		public T GetOrCreateDataUnderLock<T, TState>(Key<T> key, TState state, Func<TState, T> factory)
		where T : class
			=> _userDataHolder.GetOrCreateDataUnderLock(key, state, factory);

		public IEnumerable<KeyValuePair<object, object>> EnumerateData()
			=> _userDataHolder.EnumerateData();

		/// <summary>Disposes this instance.</summary>
		/// <remarks>Does not implement <see cref="IDisposable"/>, is called when the lifetime is terminated.</remarks>
		private void Dispose() {
			_isValid = false;

			// Removes the references.
			IAssemblyCookie[] assemblyCookies = _assemblyReferences.Values.ToArray();
			if (assemblyCookies.Length > 0) {
				_shellLocks.ExecuteWithWriteLock(() => {
					foreach (IAssemblyCookie assemblyCookie in assemblyCookies)
						assemblyCookie.Dispose();
				});
				_assemblyReferences.Clear();
			}

			_resolveProject.Dispose();
		}

		private void AddBaseReferences() {
			TryAddReference("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			TryAddReference("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			foreach (string assemblyName in _t4Environment.TextTemplatingAssemblyNames)
				TryAddReference(assemblyName);
		}

		public T4PsiModule(
			[NotNull] Lifetime lifetime,
			[NotNull] IPsiModules psiModules,
			[NotNull] DocumentManager documentManager,
			[NotNull] ChangeManager changeManager,
			[NotNull] IAssemblyFactory assemblyFactory,
			[NotNull] IShellLocks shellLocks,
			[NotNull] IProjectFile projectFile,
			[NotNull] T4FileDataCache fileDataCache,
			[NotNull] T4Environment t4Environment,
			[NotNull] OutputAssemblies outputAssemblies
		) {

			_lifetime = lifetime;
			lifetime.OnTermination(Dispose);
			
			_psiModules = psiModules;
			_assemblyFactory = assemblyFactory;
			_changeManager = changeManager;
			_shellLocks = shellLocks;

			_projectFile = projectFile;
			IProject project = projectFile.GetProject();
			Assertion.AssertNotNull(project, "project != null");
			_project = project;
			_solution = project.GetSolution();

			changeManager.RegisterChangeProvider(lifetime, this);
			changeManager.AddDependency(lifetime, psiModules, this);

			_t4Environment = t4Environment;
			_outputAssemblies = outputAssemblies;
			_resolveProject = new T4ResolveProject(lifetime, _solution, _shellLocks, t4Environment.TargetFrameworkId, project);

			_moduleReferenceResolveContext = new PsiModuleResolveContext(this, _t4Environment.TargetFrameworkId, project);
			SourceFile = CreateSourceFile(projectFile, documentManager);

			_isValid = true;
			fileDataCache.FileDataChanged.Advise(lifetime, OnDataFileChanged);
			AddBaseReferences();
		}

	}

}