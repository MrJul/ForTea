using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Web.Generation;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4GeneratedDocumentService {

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

	}

}