using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Impl.PsiManagerImpl;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Specialization of <see cref="SecondaryDocumentGenerationResult"/> that add dependencies between a file and its includes.
	/// </summary>
	public sealed class T4SecondaryDocumentGenerationResult : SecondaryDocumentGenerationResult {
		private readonly OneToSetMap<FileSystemPath, FileSystemPath> _includedFiles;
		private readonly FileDependency _fileDependency;

		public override void CommitChanges(ISolution solution) {
			base.CommitChanges(solution);
			_fileDependency.UpdateDependencies(_includedFiles);
		}

		public T4SecondaryDocumentGenerationResult([NotNull] IPsiSourceFile sourceFile, [NotNull] string text, [NotNull] PsiLanguageType language,
			[NotNull] ISecondaryRangeTranslator secondaryRangeTranslator, [NotNull] ILexerFactory lexerFactory,
			[NotNull] FileDependency fileDependency, [NotNull] OneToSetMap<FileSystemPath, FileSystemPath> includedFiles)
			: base(sourceFile, text, language, secondaryRangeTranslator, lexerFactory) {
			_fileDependency = fileDependency;
			_includedFiles = includedFiles;
		}

	}

}