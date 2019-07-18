using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Collections;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Web.Impl.PsiModules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules {

	/// <summary>PSI module managing a single T4 file.</summary>
	internal sealed class T4FilePsiModule : ProjectPsiModuleBase, IT4FilePsiModule
	{
		private readonly Lifetime _lifetime;
		[NotNull] private readonly T4AssemblyReferenceManager _assemblyReferenceManager;
		[NotNull] private readonly IPsiModules _psiModules;
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly OutputAssemblies _outputAssemblies;
		[NotNull] private readonly IT4MacroResolver _resolver;
		
		[NotNull] private readonly Dictionary<string, string> _resolvedMacros
			= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		[NotNull]
		private IProjectFile File { get; }
		
		private IChangeProvider ChangeProvider { get; } = new FakeChangeProvider();

		/// <summary>
		/// Gets the language used by this PSI module.
		/// This should be the code behind language, not the primary language.
		/// </summary>
		public override PsiLanguageType PsiLanguage => T4Language.Instance;

		/// <summary>
		/// Gets the project file type used by this PSI module:
		/// always <see cref="JetBrains.ProjectModel.ProjectFileType"/>.
		/// </summary>
		public override ProjectFileType ProjectFileType => T4ProjectFileType.Instance;
		
		/// <summary>Returns the source file associated with this PSI module.</summary>
		[NotNull]
		public IPsiSourceFile SourceFile { get; }

		[NotNull]
		[ItemNotNull]
		protected override IEnumerable<IPsiSourceFile> GetSourceFiles() => new[] {SourceFile};

		private void OnDataFileChanged(Pair<IPsiSourceFile, T4FileDataDiff> pair) {
			(IPsiSourceFile first, T4FileDataDiff second) = pair;

			if (first != SourceFile)
				return;

			if (_shellLocks.IsWriteAccessAllowed())
				OnDataFileChanged(second);
			else {
				_shellLocks.ExecuteOrQueueEx(_lifetime, "T4PsiModuleOnFileDataChanged",
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
				File,
				_assemblyReferenceManager
			);
			
			if (!hasChanges)
				return;

			// tells the world the module has changed
			var changeBuilder = new PsiModuleChangeBuilder();
			changeBuilder.AddModuleChange(this, PsiModuleChange.ChangeType.Modified);

			if (hasMacroChanges)
				GetPsiServices().MarkAsDirty(SourceFile);

			_shellLocks.ExecuteOrQueueEx("T4PsiModuleChange",
				() => _changeManager.ExecuteAfterChange(
					() => _shellLocks.ExecuteWithWriteLock(
						() => _changeManager.OnProviderChanged(ChangeProvider, changeBuilder.Result, SimpleTaskExecutor.Instance)
					)
				)
			);
		}

		/// <summary>Resolves new VS macros, like $(SolutionDir), found in include or assembly directives.</summary>
		/// <param name="macros">The list of macro names (eg SolutionDir) to resolve.</param>
		/// <returns>Whether at least one macro has been processed.</returns>
		private bool ResolveMacros([NotNull] IEnumerable<string> macros)
		{
			var result = _resolver.Resolve(macros, File);

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
		protected override IEnumerable<IPsiModuleReference> GetReferencesInternal() {
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

		[NotNull]
		private PsiProjectFile CreateSourceFile([NotNull] IProjectFile projectFile, [NotNull] DocumentManager documentManager)
			=> new PsiProjectFile(
				this,
				projectFile,
				(pf, sf) => new T4PsiProjectFileProperties(pf, sf, true),
				JetFunc<IProjectFile, IPsiSourceFile>.True,
				documentManager,
				_assemblyReferenceManager.ResolveContext
			);

		/// <summary>Disposes this instance.</summary>
		/// <remarks>Does not implement <see cref="IDisposable"/>, is called when the lifetime is terminated.</remarks>
		private void Dispose()
		{
			// Removes the references.
			IAssemblyCookie[] assemblyCookies = _assemblyReferenceManager.References.Values.ToArray();

			if (assemblyCookies.Length <= 0) return;

			_shellLocks.ExecuteWithWriteLock(() =>
			{
				foreach (IAssemblyCookie assemblyCookie in assemblyCookies)
				{
					assemblyCookie.Dispose();
				}
			});
			_assemblyReferenceManager.References.Clear();
		}

		private void AddBaseReferences() {
			_assemblyReferenceManager.TryAddReference("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			_assemblyReferenceManager.TryAddReference("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			foreach (string assemblyName in _t4Environment.TextTemplatingAssemblyNames)
				_assemblyReferenceManager.TryAddReference(assemblyName);
		}

		public T4FilePsiModule(
			Lifetime lifetime,
			[NotNull] IProjectFile file,
			[NotNull] ChangeManager changeManager,
			[NotNull] IShellLocks shellLocks,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IT4MacroResolver resolver,
			[NotNull] PsiProjectFileTypeCoordinator coordinator
		) : base(
			file.GetProject().NotNull(),
			file.Location.TryMakeRelativeTo(file.GetProject().NotNull().Location).FullPath,
			coordinator,
			t4Environment.TargetFrameworkId
		)
		{
			File = file;
			_lifetime = lifetime;
			lifetime.OnTermination(Dispose);

			var solution = file.GetSolution();
			_psiModules = solution.GetComponent<IPsiModules>();
			_changeManager = changeManager;
			_shellLocks = shellLocks;
			_t4Environment = t4Environment;
			_resolver = resolver;

			var resolveContext = this.GetResolveContextEx(file);

			_assemblyReferenceManager = new T4AssemblyReferenceManager(
				solution.GetComponent<IAssemblyFactory>(),
				file,
				resolveContext
			);

			changeManager.RegisterChangeProvider(lifetime, ChangeProvider);
			changeManager.AddDependency(lifetime, _psiModules, ChangeProvider);

			_outputAssemblies = solution.GetComponent<OutputAssemblies>();

			var documentManager = solution.GetComponent<DocumentManager>();
			SourceFile = CreateSourceFile(file, documentManager);

			solution.GetComponent<T4FileDataCache>().FileDataChanged.Advise(lifetime, OnDataFileChanged);
			AddBaseReferences();
		}
	}
}
