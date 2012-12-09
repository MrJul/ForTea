using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.Common;
using JetBrains.Util;
using PlatformID = JetBrains.ProjectModel.PlatformID;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// This almost empty implementation of <see cref="IProject"/> has only one purpose: provide a custom <see cref="PlatformID"/>
	/// so that the GAC resolver will correctly resolve from the profile we want, which is the full framework.
	/// TODO: see with JetBrains if that can be changed, this is really ugly
	/// </summary>
	internal sealed class T4ResolveProject : IProject {

		private readonly IUserDataHolder _dataHolder;
		private readonly Guid _guid = Guid.NewGuid();
		private readonly ISolution _solution;
		private readonly IShellLocks _shellLocks;
		private readonly IProjectProperties _projectProperties;


		private sealed class T4ResolveProjectProperties : ProjectPropertiesBase {

			public override IBuildSettings BuildSettings {
				get { return null; }
			}

			public override ProjectLanguage DefaultLanguage {
				get { return null; }
			}

			public override ProjectKind ProjectKind {
				get { return ProjectKind.MISC_FILES_PROJECT; }
			}

			internal T4ResolveProjectProperties([NotNull] PlatformID platformID)
				: base(EmptyList<Guid>.InstanceList, platformID, Guid.Empty) {
			}

		}

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
		
		void IProjectItem.Dump(TextWriter to) {
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
		
		ICollection<IModuleToModuleReference> IModule.GetReferences() {
			return EmptyList<IModuleToModuleReference>.InstanceList;
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
		
		IEnumerable<IProjectToModuleReference> IProject.GetModuleReferences() {
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

		bool IProject.IsWebProject {
			get { return false; }
		}

		bool IProject.IsWebApplication {
			get { return false; }
		}

		ProjectKind IProject.ProjectKind {
			get { return ProjectKind.MISC_FILES_PROJECT; }
		}

		ProjectLanguage IProject.DefaultLanguage {
			get { return null; }
		}

		IBuildSettings IProject.BuildSettings {
			get { return null; }
		}

		IProjectConfiguration IProject.ActiveConfiguration {
			get { return null; }
		}

		IProjectProperties IProject.ProjectProperties {
			get { return _projectProperties; }
		}

		FileSystemPath IProject.ProjectFileLocation {
			get { return null; }
		}


		internal T4ResolveProject([NotNull] ISolution solution, [NotNull] IShellLocks shellLocks, [NotNull] PlatformID platformID, [NotNull] IUserDataHolder dataHolder) {
			_shellLocks = shellLocks;
			_solution = solution;
			_dataHolder = dataHolder;
			_projectProperties = new T4ResolveProjectProperties(platformID);
		}

	}

}