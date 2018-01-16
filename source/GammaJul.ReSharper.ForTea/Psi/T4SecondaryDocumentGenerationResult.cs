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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Specialization of <see cref="SecondaryDocumentGenerationResult"/> that add dependencies between a file and its includes.
	/// </summary>
	public sealed class T4SecondaryDocumentGenerationResult : SecondaryDocumentGenerationResult {

		[NotNull] private readonly IPsiSourceFile _sourceFile;
		[NotNull] private readonly HashSet<FileSystemPath> _includedFiles;
		[NotNull] private readonly T4FileDependencyManager _t4FileDependencyManager;

		public override void CommitChanges() {
			base.CommitChanges();

			FileSystemPath location = _sourceFile.GetLocation();
			if (!location.IsEmpty) {
				_t4FileDependencyManager.UpdateIncludes(location, _includedFiles);
				_t4FileDependencyManager.TryGetCurrentInvalidator()?.AddCommittedFilePath(location);
			}
		}

		// ReSharper disable once UnusedParameter.Local
		public T4SecondaryDocumentGenerationResult([NotNull] IPsiSourceFile sourceFile, [NotNull] string text, [NotNull] PsiLanguageType language,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator, [NotNull] ILexerFactory lexerFactory,
			[NotNull] T4FileDependencyManager t4FileDependencyManager, [NotNull] IEnumerable<FileSystemPath> includedFiles)
			: base(text, language, secondaryRangeTranslator, lexerFactory) {
			_sourceFile = sourceFile;
			_t4FileDependencyManager = t4FileDependencyManager;
			_includedFiles = new HashSet<FileSystemPath>(includedFiles);
		}

	}

}