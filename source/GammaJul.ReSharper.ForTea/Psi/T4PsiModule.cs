using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.ForTea.Common;
using GammaJul.ReSharper.ForTea.Psi;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Collections;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.Interop.WinApi;
using JetBrains.Lifetimes;
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

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>PSI module managing a single T4 file.</summary>
	internal sealed class T4PsiModule : IChangeProvider, IT4PsiModule
	{

		private const string Prefix = "[T4] ";
		private readonly Lifetime _lifetime;
		[NotNull] private readonly T4AssemblyReferenceManager _assemblyReferenceManager;
		[NotNull] private readonly Dictionary<string, string> _resolvedMacros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		[NotNull] private readonly IPsiModules _psiModules;
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly ProjectInfo _projectInfo;
		[NotNull] private readonly OutputAssemblies _outputAssemblies;
		[NotNull] private readonly UserDataHolder _userDataHolder = new UserDataHolder();
		[NotNull] private readonly IT4MacroResolver _resolver;

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
			=> _projectInfo.Project;

		/// <summary>TargetFrameworkId corresponding to the module.</summary>
		public TargetFrameworkId TargetFrameworkId
			=> _t4Environment.TargetFrameworkId;

		/// <summary>Gets the solution this PSI module is attached to.</summary>
		/// <returns>An instance of <see cref="ISolution"/>.</returns>
		public ISolution GetSolution()
			=> _projectInfo.Solution;

		/// <summary>Gets an instance of <see cref="IPsiServices"/> for the current solution.</summary>
		/// <returns>An instance of <see cref="IPsiServices"/>.</returns>
		public IPsiServices GetPsiServices()
			=> _projectInfo.Solution.GetPsiServices();

		/// <summary>Gets whether the PSI module is valid.</summary>
		/// <returns><c>true</c> if the PSI module is valid, <c>false</c> otherwise.</returns>
		public bool IsValid()
			=> _isValid;

		/// <summary>Gets a persistent identifier for this PSI module.</summary>
		/// <returns>A persistent identifier.</returns>
		public string GetPersistentID()
			=> Prefix + _projectInfo.ProjectFile.GetPersistentID();


		private void OnDataFileChanged(Pair<IPsiSourceFile, T4FileDataDiff> pair) {
			(IPsiSourceFile first, T4FileDataDiff second) = pair;

			if (first != SourceFile)
				return;

			if (_shellLocks.IsWriteAccessAllowed())
				OnDataFileChanged(second);
			else {
				_shellLocks.ExecuteOrQueue(_lifetime, "T4PsiModuleOnFileDataChanged",
					() => _shellLocks.ExecuteWithWriteLock(() => OnDataFileChanged(second)));
			}
		}

		/// <summary>Called when the associated data file changed: added/removed assemblies or includes.</summary>
		/// <param name="dataDiff">The difference between the old and new data.</param>
		private void OnDataFileChanged([NotNull] T4FileDataDiff dataDiff) {
			_shellLocks.AssertWriteAccessAllowed();

			bool hasMacroChanges = ResolveMacros(dataDiff.AddedMacros);
			bool hasChanges = hasMacroChanges;

			_resolver.InvalidateAssemblies(
				dataDiff,
				ref hasChanges,
				_projectInfo,
				_assemblyReferenceManager
			);
			
			if (!hasChanges)
				return;

			// tells the world the module has changed
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.Modified);

			if (hasMacroChanges)
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
		private bool ResolveMacros([NotNull] IEnumerable<string> macros)
		{
			var result = _resolver.Resolve(macros, _projectInfo);

			if (result.IsEmpty())
			{
				return false;
			}

			lock (_resolvedMacros)
			{
				foreach (var (key, value) in result)
				{
					_resolvedMacros[key] = value;
				}
			}

			return true;
		}

		/// <summary>Gets all modules referenced by this module.</summary>
		/// <returns>All referenced modules.</returns>
		public IEnumerable<IPsiModuleReference> GetReferences(IModuleReferenceResolveContext moduleReferenceResolveContext) {
			_shellLocks.AssertReadAccessAllowed();
			
			var references = new PsiModuleReferenceAccumulator(TargetFrameworkId);
			
			foreach (IAssemblyCookie cookie in _assemblyReferenceManager.References.Values) {
				if (cookie.Assembly == null)
					continue;

				IPsiModule psiModule = _psiModules.GetPrimaryPsiModule(cookie.Assembly, TargetFrameworkId);

				// Normal assembly.
				if (psiModule != null)
					references.Add(new PsiModuleReference(psiModule));

				// Assembly that is the output of a current project: reference the project instead.
				else {
					IProject project = _outputAssemblies.TryGetProjectByOutputAssembly(cookie.Assembly);
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
		public IDictionary<string, string> GetResolvedMacros()
		{
			lock (_resolvedMacros)
			{
				if (_resolvedMacros.IsEmpty())
					return EmptyDictionary<string, string>.Instance;

				return new Dictionary<string, string>(_resolvedMacros);
			}
		}

		object IChangeProvider.Execute(IChangeMap changeMap) => null;

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
				_assemblyReferenceManager.ModuleReferenceResolveContext
			);
		
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
			IAssemblyCookie[] assemblyCookies = _assemblyReferenceManager.References.Values.ToArray();
			if (assemblyCookies.Length > 0) {
				_shellLocks.ExecuteWithWriteLock(() => {
					foreach (IAssemblyCookie assemblyCookie in assemblyCookies)
						assemblyCookie.Dispose();
				});
				_assemblyReferenceManager.References.Clear();
			}

			_assemblyReferenceManager.Dispose();
		}

		private void AddBaseReferences() {
			_assemblyReferenceManager.TryAddReference("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			_assemblyReferenceManager.TryAddReference("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			foreach (string assemblyName in _t4Environment.TextTemplatingAssemblyNames)
				_assemblyReferenceManager.TryAddReference(assemblyName);
		}

		public T4PsiModule(
			Lifetime lifetime,
			[NotNull] IPsiModules psiModules,
			[NotNull] DocumentManager documentManager,
			[NotNull] ChangeManager changeManager,
			[NotNull] IAssemblyFactory assemblyFactory,
			[NotNull] IShellLocks shellLocks,
			[NotNull] ProjectInfo projectInfo,
			[NotNull] T4FileDataCache fileDataCache,
			[NotNull] IT4Environment t4Environment,
			[NotNull] OutputAssemblies outputAssemblies,
			[NotNull] IT4MacroResolver resolver
		)
		{
			_lifetime = lifetime;
			lifetime.OnTermination(Dispose);

			_psiModules = psiModules;
			_changeManager = changeManager;
			_shellLocks = shellLocks;
			_projectInfo = projectInfo;
			var resolveProject = new T4ResolveProject(
				lifetime,
				_projectInfo.Solution,
				_shellLocks,
				t4Environment.TargetFrameworkId,
				_projectInfo.Project
			);

			var resolveContext = new PsiModuleResolveContext(this, t4Environment.TargetFrameworkId, _projectInfo.Project);
			_assemblyReferenceManager = new T4AssemblyReferenceManager(assemblyFactory, _projectInfo, resolveProject, resolveContext);

			changeManager.RegisterChangeProvider(lifetime, this);
			changeManager.AddDependency(lifetime, psiModules, this);

			_t4Environment = t4Environment;
			_outputAssemblies = outputAssemblies;

			SourceFile = CreateSourceFile(_projectInfo.ProjectFile, documentManager);

			_isValid = true;
			fileDataCache.FileDataChanged.Advise(lifetime, OnDataFileChanged);
			AddBaseReferences();

			_resolver = resolver;
		}
	}
}