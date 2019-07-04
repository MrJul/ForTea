using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {

	/// <summary>Specialization of <see cref="SecondaryDocumentGenerationResult"/> that add dependencies between a file and its includes.</summary>
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

		public T4SecondaryDocumentGenerationResult(
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] string text,
			[NotNull] PsiLanguageType language,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator,
			[NotNull] ILexerFactory lexerFactory,
			[NotNull] T4FileDependencyManager t4FileDependencyManager,
			[NotNull] IEnumerable<FileSystemPath> includedFiles
		)
			: base(text, language, secondaryRangeTranslator, lexerFactory) {
			_sourceFile = sourceFile;
			_t4FileDependencyManager = t4FileDependencyManager;
			_includedFiles = new HashSet<FileSystemPath>(includedFiles);
		}

	}

}