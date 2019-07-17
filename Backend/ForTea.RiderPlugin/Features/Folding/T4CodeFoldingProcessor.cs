using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
	public class T4CodeFoldingProcessor : ICodeFoldingProcessor
	{
		private bool FileProcessed { get; set; }
		public bool InteriorShouldBeProcessed(ITreeNode element, FoldingHighlightingConsumer context) => false;
		public bool IsProcessingFinished(FoldingHighlightingConsumer context) => false;

		public void ProcessBeforeInterior(ITreeNode element, FoldingHighlightingConsumer context)
		{
			if (!FileProcessed && element.GetContainingFile() is IT4File file) CollectFileFoldings(context, file);
			FileProcessed = true;

			if (element is T4CodeBlock && !(element is T4FeatureBlock))
				context.AddDefaultPriorityFolding(
					T4CodeFoldingAttributes.Directive,
					element.GetDocumentRange(),
					"<# ... #>");
		}

		private void CollectFileFoldings(FoldingHighlightingConsumer context, IT4File file)
		{
			foreach (var range in FindDirectiveFoldings(file))
			{
				context.AddDefaultPriorityFolding(T4CodeFoldingAttributes.Directive, range, "<#@ ... #>");
			}

			var featureFolding = FindFeatureFoldings(file);
			if (featureFolding.HasValue)
			{
				context.AddDefaultPriorityFolding(
					T4CodeFoldingAttributes.Directive,
					featureFolding.Value,
					"<#+ ... #>");
			}
		}

		[NotNull]
		private IEnumerable<DocumentRange> FindDirectiveFoldings([NotNull] IT4File file)
		{
			int? directiveStart = null;
			int? directiveEnd = null;

			foreach (var node in file.Children())
			{
				if (node is IT4Directive)
				{
					directiveStart = directiveStart ?? node.GetTreeStartOffset().Offset;
					directiveEnd = node.GetTreeEndOffset().Offset;
					continue;
				}

				if (node.NodeType == T4TokenNodeTypes.NEW_LINE) continue;
				if (directiveStart == null) continue;

				yield return new DocumentRange(
					file.GetSourceFile().NotNull().Document,
					new TextRange(directiveStart.Value, directiveEnd.Value));
				directiveStart = null;
				directiveEnd = null;
			}

			if (directiveStart == null) yield break;
			yield return new DocumentRange(
				file.GetSourceFile().NotNull().Document,
				new TextRange(directiveStart.Value, directiveEnd.Value));
		}

		private DocumentRange? FindFeatureFoldings([NotNull] IT4File file)
		{
			int? start = file.Children().OfType<T4FeatureBlock>().FirstOrDefault()?.GetTreeStartOffset().Offset;
			int? end = file.Children().OfType<T4FeatureBlock>().LastOrDefault()?.GetTreeEndOffset().Offset;
			if (!start.HasValue || !end.HasValue) return null;
			return new DocumentRange(
				file.GetSourceFile().NotNull().Document,
				new TextRange(start.Value, end.Value));
		}

		public void ProcessAfterInterior(ITreeNode element, FoldingHighlightingConsumer context)
		{
		}
	}
}
