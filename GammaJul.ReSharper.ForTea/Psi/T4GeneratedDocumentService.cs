#region License
//    Copyright 2012 Julien Lebosquain
//    Copyright 2016 Caelan Sayler - [caelantsayler]at[gmail]com
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
using JetBrains.DocumentModel;
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB;
using JetBrains.ReSharper.Psi.Web.Generation;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// This class will generate a code-behind from a T4 file.
	/// </summary>
	[GeneratedDocumentService(typeof(T4ProjectFileType))]
	public class T4GeneratedDocumentService : GeneratedDocumentServiceBase {

		[NotNull] private readonly DirectiveInfoManager _directiveInfoManager;
		
		/// <summary>
		/// Generates a C# or VB file from a T4 file.
		/// </summary>
		/// <param name="modificationInfo">The modifications that occurred in the T4 file.</param>
		public override ISecondaryDocumentGenerationResult Generate(PrimaryFileModificationInfo modificationInfo) {
			var t4File = modificationInfo.NewPsiFile as IT4File;
			if (t4File == null)
				return null;

			var templateNode = t4File.FindNextNode(n =>
			{
				var directive = n as IT4Directive;
				if (directive != null && directive.IsSpecificDirective(_directiveInfoManager.Template))
					return TreeNodeActionType.ACCEPT;

				return TreeNodeActionType.CONTINUE;
			}) as IT4Directive;

			if (templateNode == null)
				return null;

			var templateLang = templateNode.GetAttributeValue(_directiveInfoManager.Template.LanguageAttribute.Name);

			GenerationResult result;
			LanguageService langService;
			if (templateLang != null && templateLang.StartsWith("VB", StringComparison.OrdinalIgnoreCase))
			{
				result = new T4VBCodeGenerator(t4File, _directiveInfoManager).Generate();
				langService = VBLanguage.Instance.LanguageService();
			}
			else {
				result = new T4CSharpCodeGenerator(t4File, _directiveInfoManager).Generate();
				langService = CSharpLanguage.Instance.LanguageService();
			}

			if (result == null || langService == null)
				return null;

			var includedFiles = new OneToSetMap<FileSystemPath, FileSystemPath>();
			includedFiles.AddRange(modificationInfo.SourceFile.GetLocation(), t4File.GetNonEmptyIncludePaths());

			ISolution solution = modificationInfo.SourceFile.GetSolution();
			var t4FileDependencyManager = solution.GetComponent<T4FileDependencyManager>();

			return new T4SecondaryDocumentGenerationResult(
				modificationInfo.SourceFile,
				result.Builder.ToString(),
				langService.LanguageType,
				new RangeTranslatorWithGeneratedRangeMap(result.GeneratedRangeMap),
				langService.GetPrimaryLexerFactory(),
				t4FileDependencyManager,
				t4File.GetNonEmptyIncludePaths());
		}

		/// <summary>
		/// Gets the secondary PSI language types for a T4 file.
		/// </summary>
		/// <returns>Always <see cref="CSharpLanguage"/>.</returns>
		public override IEnumerable<PsiLanguageType> GetSecondaryPsiLanguageTypes(IProject project) {
			return new PsiLanguageType[] { CSharpLanguage.Instance, VBLanguage.Instance };
		}

		public override bool IsSecondaryPsiLanguageType(IProject project, PsiLanguageType language) {
			return language.Is<CSharpLanguage>() || language.Is<VBLanguage>();
		}

		/// <summary>
		/// Creates a secondary lexing service for code behind generated files.
		/// </summary>
		/// <param name="solution">The solution.</param>
		/// <param name="mixedLexer">The mixed lexer.</param>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>An instance of <see cref="ISecondaryLexingProcess"/> used to lex the code behind file.</returns>
		public override ISecondaryLexingProcess CreateSecondaryLexingService(ISolution solution, MixedLexer mixedLexer, IPsiSourceFile sourceFile = null) {
			return CSharpLanguage.Instance != null ? new T4SecondaryLexingProcess(CSharpLanguage.Instance, mixedLexer) : null;
		}

		/// <summary>
		/// Gets a lexer factory capable of handling preprocessor directives.
		/// </summary>
		/// <param name="primaryLanguage">The primary language.</param>
		/// <returns>Always <c>null</c> since there is no preprocessor directives in T4 files.</returns>
		public override ILexerFactory LexerFactoryWithPreprocessor(PsiLanguageType primaryLanguage) {
			return null;
		}

		/// <summary>
		/// Reparses the original T4 file.
		/// </summary>
		/// <param name="treeTextRange">The tree text range to reparse.</param>
		/// <param name="newText">The new text to add at <paramref name="treeTextRange"/>.</param>
		/// <param name="rangeTranslator">The range translator.</param>
		/// <returns><c>true</c> if reparse succeeded, <c>false</c> otherwise.</returns>
		protected override bool ReparseOriginalFile(TreeTextRange treeTextRange, string newText, RangeTranslatorWithGeneratedRangeMap rangeTranslator) {
			var t4File = rangeTranslator.OriginalFile as IT4File;
			return t4File != null && t4File.ReParse(treeTextRange, newText) != null;
		}
		
		/// <summary>
		/// The process of generated document commit (in the case of primary document incremental reparse) can be overridden in this method.
		/// Returns null if full regeneration is required.
		/// This method is not allowed to do destructive changes due to interruptibility!
		/// </summary>
		public override ICollection<ICommitBuildResult> ExecuteSecondaryDocumentCommitWork(PrimaryFileModificationInfo primaryFileModificationInfo,
			CachedPsiFile cachedPsiFile, TreeTextRange oldTreeRange, string newText) {
			var rangeTranslator = (RangeTranslatorWithGeneratedRangeMap) cachedPsiFile.PsiFile.SecondaryRangeTranslator;
			if (rangeTranslator == null)
				return null;

			TreeTextRange range = rangeTranslator.OriginalToGenerated(oldTreeRange, JetPredicate<IUserDataHolder>.True);
			DocumentRange documentRange = cachedPsiFile.PsiFile.DocumentRangeTranslator.Translate(range);
			if (!documentRange.IsValid())
				return null;

			var documentChange = new DocumentChange(documentRange.Document, documentRange.TextRange.StartOffset, documentRange.TextRange.Length, newText,
				documentRange.Document.LastModificationStamp, TextModificationSide.NotSpecified);

			return new ICommitBuildResult[] {
				new CommitBuildResult(cachedPsiFile.WorkIncrementalParse(documentChange), null, documentChange, null, TextRange.InvalidRange, string.Empty),
				new FixRangeTranslatorsOnSharedRangeCommitBuildResult(rangeTranslator, null, new TreeTextRange<Original>(oldTreeRange), new TreeTextRange<Generated>(range), newText)
			};
		}
		
		public T4GeneratedDocumentService([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}