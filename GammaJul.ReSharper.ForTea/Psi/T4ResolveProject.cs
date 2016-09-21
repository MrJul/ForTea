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


using JetBrains.DataFlow;
using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ProjectModel.ProjectImplementation;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.Common;
using JetBrains.ProjectModel.References;
using JetBrains.Util;
using PlatformID = JetBrains.Application.platforms.PlatformID;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// This almost empty implementation of <see cref="IProject"/> has only one purpose: provide a custom <see cref="PlatformID"/>
	/// so that the GAC resolver will correctly resolve from the profile we want, which is the full framework.
	/// TODO: see with JetBrains if that can be changed, this is really ugly
	/// </summary>
	internal sealed class T4ResolveProject : IProject {

		private readonly Guid _guid = Guid.NewGuid();
		[NotNull] private readonly IUserDataHolder _dataHolder;
		[NotNull] private readonly ISolution _solution;
		[NotNull] private readonly IShellLocks _shellLocks;
		[NotNull] private readonly IProjectProperties _projectProperties;
		[NotNull] private readonly IProperty<FileSystemPath> _projectFileLocationLive;
		[NotNull] private readonly IProperty<FileSystemPath> _projectLocationLive;
		[NotNull] private readonly TargetFrameworkReferences _targetFrameworkReferences;

		public void PutData<T>(Key<T> key, T val) where T : class {
			_dataHolder.PutData(key, val);
		}

		public T GetData<T>(Key<T> key) where T : class {
			return _dataHolder.GetData(key);
		}

		public IEnumerable<KeyValuePair<object, object>> EnumerateData() {
			return _dataHolder.EnumerateData();
		}

		public string Name {
			get { return GetType().FullName; }
		}

		public bool IsValid() {
			return _solution.IsValid();
		}

		public ISolution GetSolution() {
			return _solution;
		}

		void IProjectModelElement.Accept(ProjectVisitor projectVisitor) {
		}

		object IProjectModelElement.GetProperty(Key propertyName) {
			return _dataHolder.GetData(propertyName);
		}

		void IProjectModelElement.SetProperty(Key propertyName, object propertyValue) {
			_dataHolder.PutData(propertyName, propertyValue);
		}
		
		IProject IProjectElement.GetProject() {
			return this;
		}

		string IProjectElement.GetPersistentID() {
			return Name;
		}

		void IProjectItem.Dump(TextWriter to, DumpFlags flags) {
		}

		string IProjectItem.GetPresentableProjectPath() {
			return null;
		}

		IProjectFolder IProjectItem.ParentFolder {
			get { return null; }
		}

		FileSystemPath IProjectItem.Location {
			get { return null; }
		}

		ProjectItemKind IProjectItem.Kind {
			get { return ProjectItemKind.PROJECT; }
		}

		bool IProjectItem.IsLinked {
			get { return false; }
		}

		IShellLocks IProjectItem.Locks {
			get { return _shellLocks; }
		}
		
		IProjectItem IProjectFolder.FindProjectItemByLocation(FileSystemPath location) {
			return null;
		}

		IProjectItem IProjectFolder.GetSubItem(string name) {
			return null;
		}

		IList<IProjectItem> IProjectFolder.GetSubItems(string name) {
			return EmptyList<IProjectItem>.InstanceList;
		}

		IList<IProjectItem> IProjectFolder.GetSubItems() {
			return EmptyList<IProjectItem>.InstanceList;
		}

		bool IProjectFolder.WriteProjectFolder(BinaryWriter writer, ProjectSerializationIndex index, FileSystemPath baseLocation) {
			return false;
		}

		ProjectFolderPath IProjectFolder.Path {
			get { return null; }
		}

		string IModule.Presentation {
			get { return Name; }
		}

		/// <summary>
		/// The platform to which the module is targeted. For real project is never null.
		/// </summary>
		public PlatformID PlatformID {
			get { return _projectProperties.PlatformId; }
		}
		
		public void Dispose() {
		}

		IEnumerable<IProjectToModuleReference> IProject.GetModuleReferences(TargetFrameworkId targetFrameworkId) {
			return EmptyList<IProjectToModuleReference>.InstanceList;
		}

		bool IProject.HasFlavour<T>() {
			return false;
		}

		IProjectFile IProject.ProjectFile {
			get { return null; }
		}

		Guid IProject.Guid {
			get { return _guid; }
		}

		bool IProject.IsOpened { get; set; }

		IProjectProperties IProject.ProjectProperties {
			get { return _projectProperties; }
		}

		FileSystemPath IProject.ProjectFileLocation {
			get { return null; }
		}

		FileSystemPath IProject.GetOutputDirectory(TargetFrameworkId targetFrameworkId) {
			return FileSystemPath.Empty;
		}

		FileSystemPath IProject.GetOutputFilePath(TargetFrameworkId targetFrameworkId) {
			return FileSystemPath.Empty;
		}

		public T GetComponent<T>() where T : class {
			return _solution.GetComponent<T>();
		}

		public IProperty<FileSystemPath> ProjectFileLocationLive {
			get { return _projectFileLocationLive; }
		}

		public IProperty<FileSystemPath> ProjectLocationLive {
			get { return _projectLocationLive; }
		}

		public TargetFrameworkScope GetTargetFramework(TargetFrameworkId targetFrameworkId) {
			return _targetFrameworkReferences.GetScope(targetFrameworkId);
		}

		public IEnumerable<TargetFrameworkId> TargetFrameworkIds {
			get { return new[] { TargetFrameworkId.Default }; }
		}

		private sealed class T4ResolveProjectProperties : ProjectPropertiesBase<UnsupportedProjectConfiguration>, IProjectProperties {

			public override IBuildSettings BuildSettings {
				get { return null; }
			}

			public ProjectLanguage DefaultLanguage {
				get { return ProjectLanguage.UNKNOWN; }
			}

			public ProjectKind ProjectKind {
				get { return ProjectKind.MISC_FILES_PROJECT; }
			}
			
			internal T4ResolveProjectProperties([NotNull] PlatformID platformID)
				: base(EmptyList<Guid>.InstanceList, platformID, Guid.Empty) {
			}

		}
		
		internal T4ResolveProject([NotNull] Lifetime lifetime, [NotNull] ISolution solution, [NotNull] IShellLocks shellLocks,
			[NotNull] PlatformID platformID, [NotNull] IUserDataHolder dataHolder) {
			_shellLocks = shellLocks;
			_solution = solution;
			_dataHolder = dataHolder;
			_projectProperties = new T4ResolveProjectProperties(platformID);
			_projectFileLocationLive = new Property<FileSystemPath>(lifetime, "ProjectFileLocationLive");
			_projectLocationLive = new Property<FileSystemPath>(lifetime, "ProjectLocationLive");
			_targetFrameworkReferences = new TargetFrameworkReferences(lifetime, shellLocks, this, solution.SolutionOwner.GetComponent<AssemblyInfoDatabase>());
		}

	}

}