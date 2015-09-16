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


using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Manages the include dependencies between T4 files, to be able to refresh includers when the included files change.
	/// </summary>
	[PsiComponent]
	public partial class T4FileDependencyManager {

		[NotNull] private readonly object _locker = new object();
		[NotNull] private readonly OneToSetMap<FileSystemPath, FileSystemPath> _includerToIncludees = new OneToSetMap<FileSystemPath, FileSystemPath>();
		[NotNull] private readonly OneToSetMap<FileSystemPath, FileSystemPath> _includeeToIncluders = new OneToSetMap<FileSystemPath, FileSystemPath>();
		[NotNull] private readonly IPsiServices _psiServices;
		[CanBeNull] private Invalidator _invalidator;

		public void UpdateIncludes([NotNull] FileSystemPath includer, [NotNull] ICollection<FileSystemPath> includees) {
			lock (_locker) {

				foreach (FileSystemPath includee in _includerToIncludees[includer])
					_includeeToIncluders.Remove(includee, includer);
				_includerToIncludees.RemoveKey(includer);

				if (includees.Count > 0) {
					_includerToIncludees.AddRangeFast(includer, includees);
					foreach (FileSystemPath includee in includees)
						_includeeToIncluders.Add(includee, includer);
				}
			}
		}
		
		[CanBeNull]
		public IT4FileDependencyInvalidator TryGetCurrentInvalidator() {
			lock (_locker)
				return _invalidator;
		}
		
		[NotNull]
		public HashSet<FileSystemPath> GetIncluders([NotNull] FileSystemPath includee) {
			lock (_locker)
				return new HashSet<FileSystemPath>(_includeeToIncluders[includee]);
		}

		private void OnAfterFilesCommit() {
			Invalidator invalidator;
			lock (_locker) {
				invalidator = _invalidator;
				_invalidator = null;
			}

			invalidator?.CommitNeededDocuments();
		}

		private void OnBeforeFilesCommit() {
			var invalidator = new Invalidator(this, _psiServices);
			lock (_locker)
				_invalidator = invalidator;
		}

		public T4FileDependencyManager([NotNull] Lifetime lifetime, [NotNull] IPsiServices psiServices) {
			_psiServices = psiServices;

			psiServices.Files.ObserveBeforeCommit(lifetime, OnBeforeFilesCommit);
			psiServices.Files.ObserveAfterCommit(lifetime, OnAfterFilesCommit);
		}

	}

	

}