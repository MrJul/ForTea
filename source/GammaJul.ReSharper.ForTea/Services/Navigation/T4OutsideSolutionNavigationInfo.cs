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
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	public class T4OutsideSolutionNavigationInfo {

		private readonly FileSystemPath _fileSystemPath;
		private readonly DocumentRange _documentRange;
		private readonly bool _activate;
		private readonly TabOptions _tabOptions;

		[NotNull]
		public FileSystemPath FileSystemPath {
			get { return _fileSystemPath; }
		}

		public DocumentRange DocumentRange {
			get { return _documentRange; }
		}

		public bool Activate {
			get { return _activate; }
		}

		public TabOptions TabOptions {
			get { return _tabOptions; }
		}

		public T4OutsideSolutionNavigationInfo([NotNull] FileSystemPath fileSystemPath, DocumentRange documentRange, bool activate, TabOptions tabOptions) {
			_fileSystemPath = fileSystemPath;
			_documentRange = documentRange;
			_activate = activate;
			_tabOptions = tabOptions;
		}

	}

}