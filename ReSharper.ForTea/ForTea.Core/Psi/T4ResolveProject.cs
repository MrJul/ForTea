using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using JetBrains.Application.Infra;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.ProjectImplementation;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.Common;
using JetBrains.ProjectModel.References;
using JetBrains.Util;
using JetBrains.Util.dataStructures;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Psi {

	/// <summary>
	/// This almost empty implementation of <see cref="IProject"/> has only one purpose: provide a custom <see cref="PlatformID"/>
	/// so that the GAC resolver will correctly resolve from the profile we want, which is the full framework.
	/// TODO: see with JetBrains if that can be changed, this is really ugly
	/// </summary>
	public sealed class T4ResolveProject : IProject {

		[NotNull] private readonly IUserDataHolder _dataHolder;
		[NotNull] private readonly ISolution _solution;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly TargetFrameworkId _targetFrameworkId;
		[NotNull] private readonly IProjectProperties _projectProperties;
		[NotNull] private readonly TargetFrameworkReferences _targetFrameworkReferences;

		public void PutData<T>(Key<T> key, T val)
		where T : class
			=> _dataHolder.PutData(key, val);

		public T GetOrCreateDataUnderLock<T>(Key<T> key, Func<T> factory)
		where T : class
			=> _dataHolder.GetOrCreateDataUnderLock(key, factory);

		public T GetOrCreateDataUnderLock<T, TState>(Key<T> key, TState state, Func<TState, T> factory)
		where T : class
			=> _dataHolder.GetOrCreateDataUnderLock(key, state, factory);

		public T GetData<T>(Key<T> key)
		where T : class
			=> _dataHolder.GetData(key);

		public IEnumerable<KeyValuePair<object, object>> EnumerateData()
			=> _dataHolder.EnumerateData();

		public string Name
			=> typeof(T4ResolveProject).FullName ?? nameof(T4ResolveProject);

		public bool IsValid()
			=> _solution.IsValid();

		public ISolution GetSolution()
			=> _solution;

		Type IProjectModelElement.MarshallerType
			=> null;

		void IProjectModelElement.Accept(ProjectVisitor projectVisitor) {
		}

		object IProjectModelElement.GetProperty(Key propertyName)
			=> _dataHolder.GetData(propertyName);

		void IProjectModelElement.SetProperty(Key propertyName, object propertyValue)
			=> _dataHolder.PutData(propertyName, propertyValue);

		IProject IProjectElement.GetProject()
			=> this;

		string IProjectElement.GetPersistentID()
			=> Name;

		void IProjectItem.Dump(TextWriter to, DumpFlags flags) {
		}

		string IProjectItem.GetPresentableProjectPath()
			=> null;

		IProjectFolder IProjectItem.ParentFolder
			=> null;

		FileSystemPath IProjectItem.Location
			=> null;

		ProjectItemKind IProjectItem.Kind
			=> ProjectItemKind.PROJECT;

		bool IProjectItem.IsLinked
			=> false;

		IShellLocks IProjectItem.Locks
			=> _shellLocks;

		IProjectItem IProjectFolder.FindProjectItemByLocation(FileSystemPath location)
			=> null;

		IProjectItem IProjectFolder.GetSubItem(string name)
			=> null;

		ReadOnlyFrugalLocalList<IProjectItem> IProjectFolder.GetSubItems(string name)
			=> default;

		IList<IProjectItem> IProjectFolder.GetSubItems()
			=> EmptyList<IProjectItem>.InstanceList;

		ProjectFolderPath IProjectFolder.Path
			=> null;

		string IModule.Presentation
			=> Name;

		public void Dispose() {
		}

		IEnumerable<IProjectToModuleReference> IProject.GetModuleReferences(TargetFrameworkId targetFrameworkId)
			=> EmptyList<IProjectToModuleReference>.InstanceList;

		bool IProject.HasFlavour<T>()
			=> false;

		IProjectFile IProject.ProjectFile
			=> null;

		Guid IProject.Guid { get; } = Guid.NewGuid();

		bool IProject.IsOpened { get; set; }

		IProjectProperties IProject.ProjectProperties
			=> _projectProperties;

		FileSystemPath IProject.ProjectFileLocation
			=> null;

		FileSystemPath IProject.GetOutputDirectory(TargetFrameworkId targetFrameworkId)
			=> FileSystemPath.Empty;

		FileSystemPath IProject.GetOutputFilePath(TargetFrameworkId targetFrameworkId)
			=> FileSystemPath.Empty;

		public T GetComponent<T>()
		where T : class
			=> _solution.GetComponent<T>();

		[NotNull]
		public IProperty<FileSystemPath> ProjectFileLocationLive { get; }

		[NotNull]
		public IProperty<FileSystemPath> ProjectLocationLive { get; }

		public ProjectTargetFrameworkScope GetTargetFrameworkScope(TargetFrameworkId targetFrameworkId)
			=> _targetFrameworkReferences.GetScope(targetFrameworkId);

		public IEnumerable<TargetFrameworkScope> GetAllTargetFrameworks()
			=> _targetFrameworkReferences.GetAllProjectScopes();

		public IReadOnlyList<TargetFrameworkId> TargetFrameworkIds
			=> new[] { _targetFrameworkId };

		IProjectFolder IProjectFolder.GetSubFolderByPath(ProjectFolderPath projectFolderPath)
			=> null;

		ICollection<FileSystemPath> IProject.GetOutputDirectories()
			=> EmptyList<FileSystemPath>.Instance;

		ICollection<FileSystemPath> IProject.GetIntermidiateDirectories()
			=> EmptyList<FileSystemPath>.Instance;

		public FileSystemPath GetRefOutputFilePath(TargetFrameworkId targetFrameworkId)
			=> FileSystemPath.Empty;

		private sealed class T4ResolveProjectProperties : ProjectPropertiesBase<UnsupportedProjectConfiguration> {

			public override IBuildSettings BuildSettings
				=> null;

			public override ProjectLanguage DefaultLanguage
				=> ProjectLanguage.UNKNOWN;

			public override ProjectKind ProjectKind
				=> ProjectKind.MISC_FILES_PROJECT;

			public T4ResolveProjectProperties([NotNull] TargetFrameworkId targetFrameworkId)
				: base(EmptyList<Guid>.InstanceList, Guid.Empty, new[] { targetFrameworkId }, dotNetCoreSDK: null) {
			}

		}

		public T4ResolveProject(
			Lifetime lifetime,
			[NotNull] ISolution solution,
			[NotNull] IShellLocks shellLocks,
			[NotNull] TargetFrameworkId targetFrameworkId,
			[NotNull] IUserDataHolder dataHolder
		) {
			_shellLocks = shellLocks;
			_targetFrameworkId = targetFrameworkId;
			_solution = solution;
			_dataHolder = dataHolder;
			_projectProperties = new T4ResolveProjectProperties(targetFrameworkId);
			ProjectFileLocationLive = new Property<FileSystemPath>(lifetime, "ProjectFileLocationLive");
			ProjectLocationLive = new Property<FileSystemPath>(lifetime, "ProjectLocationLive");
			_targetFrameworkReferences = new TargetFrameworkReferences(lifetime, shellLocks, this, solution.SolutionOwner.GetComponent<AssemblyInfoDatabase>());
		}

	}

}