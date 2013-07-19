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
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal sealed class T4OutsideSolutionSourceFile : PsiSourceFileFromPath, IPsiSourceFileWithLocation {
		
		public FileSystemPath Location {
			get { return Path; }
		}
		
		private sealed class DocumentFactory : IDocumentFactory {

			private readonly IDocumentFactory _underlyingFactory;
			private readonly FileSystemPath _path;

			/// <summary>
			/// Creates an <see cref="T:JetBrains.DocumentModel.IDocument" /> from the given text.
			/// This is always the simple implementation of the standalone string-based document.
			/// </summary>
			/// <param name="text">Document text.</param>
			/// <param name="moniker">Document moniker that uniquely identifies this document.</param>
			/// <param name="ensureWritableHandler">Queries whether the document is allowed to be written.</param>
			public IDocument CreateSimpleDocumentFromText(string text, string moniker, Func<IDocument, ModificationCookie> ensureWritableHandler = null) {
				IDocument document = _underlyingFactory.CreateSimpleDocumentFromText(text, moniker, ensureWritableHandler);
				document.SetOutsideSolutionPath(_path);
				return document;
			}

			internal DocumentFactory([NotNull] IDocumentFactory underlyingFactory, [NotNull] FileSystemPath path) {
				_underlyingFactory = underlyingFactory;
				_path = path;
			}

		}

		public T4OutsideSolutionSourceFile(IDocumentFactory documentFactory, IProjectFileExtensions projectFileExtensions,
			PsiProjectFileTypeCoordinator projectFileTypeCoordinator, IPsiModule module, FileSystemPath path, Func<PsiSourceFileFromPath, bool> validityCheck,
			Func<PsiSourceFileFromPath, IPsiSourceFileProperties> propertiesFactory)
			: base(new DocumentFactory(documentFactory, path), projectFileExtensions, projectFileTypeCoordinator, module, path, validityCheck, propertiesFactory) {
		}

	}

}