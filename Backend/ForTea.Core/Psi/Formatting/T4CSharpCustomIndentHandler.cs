using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.Tree;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Web.CodeBehindSupport;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[ShellComponent]
	public class T4CSharpCustomIndentHandler : ICustomIndentHandler
	{
		public string Indent(
			ITreeNode node,
			CustomIndentType indentType,
			FmtSettings<CSharpFormatSettingsKey> settings
		)
		{
			var helper = new T4CSharpIndentHelper(settings.Settings);
			return IndentHandler(node, indentType, helper);
		}

		private string IndentHandler(ITreeNode node, CustomIndentType indentType, T4CSharpIndentHelper helper)
		{
			if (!node.IsPhysical()) return null;

			var file = node.GetContainingNode<IFile>(true);
			var sourceFile = file?.GetSourceFile();
			if (sourceFile?.LanguageType.Is<T4ProjectFileType>() != true) return null;

			if (indentType == CustomIndentType.RelativeNodeCalculation) return node.GetIndentViaDocument();

			if (node is ITokenNode tokenNode
			    && tokenNode.GetTokenType().IsComment
			    && tokenNode.GetText() == T4CSharpCodeBehindGenerationInfoCollector.CodeCommentEnd)
				return HandleComment(indentType, file, tokenNode);
			if (helper.IsGeneratedMethodMember(node))
			{
				var rangeTranslator = file.GetRangeTranslator();
				var originalFile = rangeTranslator.OriginalFile;
				if (indentType == CustomIndentType.RelativeLineCalculation)
				{
					var firstToken = node.GetFirstTokenIn();
					var generatedTreeRange1 = new TreeTextRange(firstToken.GetTreeStartOffset());
					var originalRange1 = rangeTranslator.GeneratedToOriginal(generatedTreeRange1);
					if (!originalRange1.IsValid())
						return null;

					var t4Element1 = originalFile.FindNodeAt(originalRange1);
					return t4Element1?.GetLineIndentFromOriginalNode(
						n => n is IT4Token token && token.GetTokenType() == T4TokenNodeTypes.RAW_CODE,
						originalRange1.StartOffset);
				}

				var statement = node as ICSharpStatement;
				if (node.GetTokenType() == CSharpTokenType.LBRACE)
					statement = node.Parent as IBlock;
				var block = BlockNavigator.GetByStatement(statement);
				if (node is IComment || node is IStartRegion || node is IEndRegion)
					block = node.GetContainingNode<ITreeNode>() as IBlock;
				if (block == null)
					return null;

				var generatedTreeRange = new TreeTextRange(node.GetTreeStartOffset());
				var blockGeneratedTreeRange = new TreeTextRange(block.GetTreeStartOffset());
				var originalRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange);
				var blockOriginalRange = rangeTranslator.GeneratedToOriginal(blockGeneratedTreeRange);
				if (!originalRange.IsValid())
					return null;

				//var originalFileText = originalFile.GetText();
				var aspElement = originalFile.FindNodeAt(originalRange);

				if (!(aspElement?.Parent?.Parent is T4CodeBlock codeBlock)) return null;
				// find previuos statement
				for (var nd = node.PrevSibling; nd != null; nd = nd.PrevSibling)
				{
					if (!helper.IsStatement(nd))
						continue;
					var token = nd.GetFirstTokenIn();
					var tmpRange = token.GetDocumentRange();
					if (!tmpRange.IsValid() || tmpRange.TextRange.EndOffset >= originalFile.GetTextLength())
						continue;

					var generatedTreeRange1 = new TreeTextRange(token.GetTreeStartOffset());
					var originalRange1 = rangeTranslator.GeneratedToOriginal(generatedTreeRange1);
					var aspElement1 = originalFile.FindNodeAt(originalRange1);
					var codeBlock1 = aspElement1?.Parent as AspRenderBlock;
					if (codeBlock1 == null || codeBlock.Parent != codeBlock1.Parent)
						break;

					return tmpRange.GetIndentFromDocumentRange();
					//var indent = GetIndent(originalFileText, tmpRange.TextRange.StartOffset, helper.TabSize);
					//return FixIndent(originalFileText, originalRange.StartOffset.Offset, helper.TabSize, indent);
				}

				// if no statement before
				if (blockOriginalRange.IsValid() && codeBlock.GetTreeTextRange().Contains(blockOriginalRange))
					return null;
				var blockStart = codeBlock.GetTreeStartOffset();
				var nodeStart = originalRange.StartOffset.Offset;
				var hasLineBreak =
					codeBlock.GetText().Substring(0, nodeStart - blockStart.Offset).IndexOf('\n') >= 0;
				//originalFileText.Substring(blockStart.Offset, nodeStart - blockStart.Offset).IndexOf('\n') >= 0;
				if (hasLineBreak)
					return "";

				return null;
			}

			if (helper.IsTypeMemberLikeNode(node, sourceFile) || node is IStartRegion || node is IEndRegion)
			{
				if (indentType == CustomIndentType.RelativeLineCalculation)
				{
					var firstToken = node.GetFirstTokenIn();
					return firstToken.GetIndentViaDocument();
				}

				var rangeTranslator = file.GetRangeTranslator();
				var generatedTreeRange = new TreeTextRange(node.GetTreeStartOffset());
				var originalTreeRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange);
				if (!originalTreeRange.IsValid()) return null;

				var t4File = rangeTranslator.OriginalFile;

				var block = t4File.FindNodeAt(originalTreeRange)?.GetContainingNode<IT4CodeBlock>(true);
				if (block == null) return null;

				// if no member before
				var blockStart = block.GetTreeStartOffset();
				var nodeStart = originalTreeRange.StartOffset.Offset;
				var hasLineBreak =
					t4File.GetText().Substring(blockStart.Offset, nodeStart - blockStart.Offset).IndexOf('\n') >= 0;
				if (hasLineBreak)
					return FormatterImplHelper.AddIndent(block.GetIndentViaDocument(), helper.IndentStr);

				return helper.IndentStr; //new string(' ', helper.TabSize);
			}

			return null;
		}

		private static string HandleComment(CustomIndentType indentType, IFile file, ITokenNode tokenNode)
		{
			if (indentType != CustomIndentType.DirectCalculation) return null;
			var rangeTranslator = file.GetRangeTranslator();
			var startOffset = tokenNode.GetTreeStartOffset();
			var generatedTreeRange = new TreeTextRange(startOffset - 1, startOffset);
			var originalTreeRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange);
			if (!originalTreeRange.IsValid()) return null;
			var t4File = rangeTranslator.OriginalFile;
			var t4Element = t4File.FindNodeAt(originalTreeRange);
			var block = t4Element?.GetContainingNode<ITreeNode>(n => n is IT4CodeBlock, true);
			return block?.GetIndentViaDocument();
		}
	}
}
