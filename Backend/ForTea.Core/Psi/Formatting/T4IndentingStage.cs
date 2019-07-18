using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Asp.Tree;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Web.CodeBehindSupport;
using JetBrains.ReSharper.Psi.Web.Util;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	public class T4IndentingStage : IndentingStage<CodeFormattingContext, T4FormatterSettingsKey>
	{
		private readonly Dictionary<ITreeNode, int> myNodeBlocks = new Dictionary<ITreeNode, int>();
		private Whitespace? myAdditionalIndentLength = null;
		private IAspCodeBlock myStartCodeBlock = null;

		public T4IndentingStage(
			CodeFormattingContext context,
			FmtSettings<T4FormatterSettingsKey> settings,
			IFormatterInfoProvider<CodeFormattingContext, T4FormatterSettingsKey> provider,
			IProgressIndicator progress,
			SequentialNodeIterator<CodeFormattingContext, T4FormatterSettingsKey> iterator)
			: base(context, settings, provider, progress, iterator)
		{
		}

		public Whitespace? CalcCustomIndent(
			ITreeNode nodeToIndent,
			Whitespace? defaultIndent,
			Whitespace settingsStr,
			IndentType indentType)
		{
			return Whitespace.Empty;
			var localBlockCount = GetAspBlockCount(nodeToIndent);

			var parentBlockCount = GetParentBlockCount(nodeToIndent);

			var nodeBlockCount = localBlockCount + parentBlockCount;

			SaveBlockCount(nodeToIndent, nodeBlockCount);
			TryRecalculateAdditionalIndent(nodeBlockCount, parentBlockCount, defaultIndent, settingsStr);

			var resStr = defaultIndent;

			if (!indentType.Has(IndentType.Alignment))
			{
				resStr = resStr + settingsStr * nodeBlockCount;
			}

			if (myAdditionalIndentLength.HasValue)
			{
				resStr += myAdditionalIndentLength.Value;
			}

			return resStr;
		}

		private int GetAspBlockCount(ITreeNode node)
		{
			return 0;
			var codeBlock = node.LeftSiblings().OfType<IAspCodeBlock>().FirstOrDefault();
			if (codeBlock == null) return 0;

			var language = codeBlock.GetCodeBehindLanguage();
			var sourceFile = codeBlock.GetSourceFile();
			if (sourceFile == null) return 0;

			var codeBehindFile = sourceFile.GetPsiServices().Files.GetDominantPsiFile(sourceFile, language);
			if (codeBehindFile == null) return 0;
			var startToken = codeBlock.StartToken;
			var endToken = codeBlock.EndToken;
			if (startToken == null || endToken == null) return 0;
			var range = new TreeTextRange(startToken.GetTreeEndOffset(), endToken.GetTreeStartOffset());
			var rangeTranslator = codeBehindFile.GetRangeTranslatorNullable();
			if (rangeTranslator == null) return 0;
			var generatedRange = rangeTranslator.OriginalToGenerated(range);
			if (!generatedRange.IsValid() || generatedRange.Length == 0) return 0;
			var token = codeBehindFile.FindTokenAt(generatedRange.EndOffset);
			if (token == null) return 0;
			var provider = Shell.Instance.GetComponent<ILanguageManager>()
				.GetService<IAspCustomFormattingInfoProvider>(language);
			var file = node.GetContainingFile();
			if (file == null) return 0;

			ITreeNode anchor = token;
			IAspCodeBlock startCodeBlock = null;

			int indent = 0;
			//bool first = true;
			while (true)
			{
				var blockInfo = provider.GetBlockInfo(anchor);
				if (!blockInfo.First.IsValid()) break;
				var originalStart =
					rangeTranslator.GeneratedToOriginal(new TreeTextRange(blockInfo.First.StartOffset, 1));
				if (!originalStart.IsValid()) break;
				var tokenStart = file.FindTokenAt(originalStart.StartOffset);
				if (tokenStart == null) break;
				var startCodeBlock1 = tokenStart.GetContainingNode<IAspCodeBlock>();
				if (startCodeBlock != null && startCodeBlock1 != startCodeBlock) break;
				if (startCodeBlock1 == null) break;
				if (startCodeBlock1.Parent != node.Parent) break;
				startCodeBlock = startCodeBlock1;
				myStartCodeBlock = startCodeBlock;
				anchor = blockInfo.Second;

				var originalEnd =
					rangeTranslator.GeneratedToOriginal(new TreeTextRange(blockInfo.First.EndOffset - 1, 1));
				if (node.GetTreeTextRange().Contains(originalEnd)) // && first)
				{
					//first = false; 
					startCodeBlock = null;
					Assertion.Assert(indent == 0, "indent == 0");
					continue;
				}

				indent++;
			}

			if (startCodeBlock == null) return 0;

			return GetAspBlockCount(startCodeBlock) + indent;
		}

		private int GetParentBlockCount(ITreeNode nodeToIndent)
		{
			var parentBlockCount = 0;
			ITreeNode parent = nodeToIndent.Parent;
			while (parent != null)
			{
				if (myNodeBlocks.ContainsKey(parent))
				{
					parentBlockCount = myNodeBlocks[parent];
					break;
				}

				parent = parent.Parent;
			}

			return parentBlockCount;
		}

		private void SaveBlockCount(ITreeNode nodeToIndent, int nodeBlockCount)
		{
			myNodeBlocks[nodeToIndent] = nodeBlockCount;
		}

		private void TryRecalculateAdditionalIndent(int nodeBlockCount, int parentBlockCount,
			Whitespace? defalutIndentLength, Whitespace settingsIndentLength)
		{
			if (myAdditionalIndentLength.HasValue) return;

			myAdditionalIndentLength = Whitespace.Empty;
			if (Context.Profile != CodeFormatProfile.SOFT) return;
			if (nodeBlockCount <= parentBlockCount) return;

			var codeBlock = myStartCodeBlock;
			codeBlock.NotNull("It wasn't null in GetAspBlockCount");
			var codeBlockIndentLength = codeBlock.GetIndentViaDocument().ToWhitespace(myTabWidth);
			var expectedCodeBlockIndentLength = defalutIndentLength + settingsIndentLength * parentBlockCount;
			myAdditionalIndentLength = codeBlockIndentLength - expectedCodeBlockIndentLength;
		}
	}
}
