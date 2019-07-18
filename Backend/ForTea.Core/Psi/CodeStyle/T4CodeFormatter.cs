using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.Text;

namespace GammaJul.ForTea.Core.Psi.CodeStyle
{
	[Language(typeof(T4Language))]
	public sealed class T4CodeFormatter : CodeFormatterBase<T4FormatterSettingsKey>
	{
		private readonly T4FormattingInfoProvider myFormattingInfoProvider;

		public T4CodeFormatter(
			[NotNull] CodeFormatterRequirements requirements,
			T4FormattingInfoProvider myFormattingInfoProvider
		) : base(T4Language.Instance, requirements) => this.myFormattingInfoProvider = myFormattingInfoProvider;

		protected override CodeFormattingContext CreateFormatterContext(
			CodeFormatProfile profile,
			ITreeNode firstNode,
			ITreeNode lastNode,
			AdditionalFormatterParameters parameters,
			ICustomFormatterInfoProvider provider
		) => new PossiblyEmbeddedCodeFormatterContext(
			this,
			firstNode,
			lastNode,
			profile,
			FormatterLoggerProvider.FormatterLogger,
			parameters
		);

		public override bool IsWhitespaceToken(ITokenNode token) => token.IsWhitespaceToken();

		protected override bool IsFormatNextSpaces(CodeFormatProfile profile) => false;

		/// <summary>
		/// Format code during WritePSI action
		/// </summary>
		public override void FormatInsertedNodes(
			ITreeNode nodeFirst,
			ITreeNode nodeLast,
			bool formatSurround)
		{
			//using (DisableIndentingInsideCommentsCookie.Create(nodeFirst.GetPsiServices()))
			{
				FormatterImplHelper.FormatInsertedNodesHelper(this, nodeFirst, nodeLast, formatSurround);
			}
		}

		/// <summary>
		/// Format code during WritePSI action
		/// </summary>
		public override ITreeRange FormatInsertedRange(ITreeNode nodeFirst, ITreeNode nodeLast, ITreeRange origin)
		{
			return FormatterImplHelper.FormatInsertedRangeHelper(this, nodeFirst, nodeLast, origin, true);
		}

		/// <summary>
		/// Format code during WritePSI action
		/// </summary>
		public override void FormatReplacedNode(ITreeNode oldNode, ITreeNode newNode)
		{
			FormatInsertedNodes(newNode, newNode, false);

			FormatterImplHelper.CheckForMinimumSeparator(this, newNode);
		}

		public override void FormatReplacedRange(ITreeNode first, ITreeNode last, ITreeRange oldNodes)
		{
			FormatInsertedNodes(first, last, false);

			FormatterImplHelper.CheckForMinimumSeparator(this, first, last);
		}

		/// <summary>
		/// Format code during WritePSI action
		/// </summary>
		public override void FormatDeletedNodes(ITreeNode parent, ITreeNode prevNode, ITreeNode nextNode)
		{
			FormatterImplHelper.FormatDeletedNodesHelper(this, parent, prevNode, nextNode, true);
		}

		public override ITokenNode GetMinimalSeparator(ITokenNode leftToken, ITokenNode rightToken)
			=> null;

		public override ITreeNode CreateSpace(string indent, ITreeNode replacedSpace)
			=> T4TokenNodeTypes.Space.Create(indent);

		public override ITreeNode CreateNewLine(LineEnding lineEnding, NodeType lineBreakType = null)
			=> T4TokenNodeTypes.NewLine.Create(lineEnding.GetPresentation());

		public override ITreeRange Format(
			ITreeNode firstElement,
			ITreeNode lastElement,
			CodeFormatProfile profile,
			AdditionalFormatterParameters parameters = null)
		{
			parameters = parameters ?? AdditionalFormatterParameters.Empty;
			var pointer = FormatterImplHelper.CreateRangePointer(firstElement, lastElement);
			FormatInternal(firstElement, lastElement, profile, parameters);
			return FormatterImplHelper.PointerToRange(pointer, firstElement, lastElement);
		}

		private void FormatInternal(ITreeNode firstElement, ITreeNode lastElement, CodeFormatProfile profile,
			AdditionalFormatterParameters parameters)
		{
			var task = new FormatTask(firstElement, lastElement, profile);
			task.Adjust(this);
			if (task.FirstElement == null) return;

			var formatterSettings = GetFormattingSettings(task.FirstElement, parameters);

			DoDeclarativeFormat(formatterSettings, myFormattingInfoProvider, null, new[] {task},
				parameters, null,
				(formatTask, settings, context) => { },
				(formatTask, settings, context) =>
				{
					using (var fmtProgress = parameters.ProgressIndicator.CreateSubProgress(1))
					{
						Assertion.Assert(formatTask.FirstElement != null, "firstNode != null");
						var file = formatTask.FirstElement.GetContainingFile();
						if (file != null)
						{
							
							RunFormatterForGeneratedLanguages(file, formatTask.FirstElement, formatTask.LastElement,
								formatTask.Profile,
								parameters.ChangeProgressIndicator(fmtProgress));
						}
					}
				}, false);
		}

		private void RunFormatterForGeneratedLanguages(
			IFile originalFile, ITreeNode firstNode, ITreeNode lastNode,
			CodeFormatProfile profile,
			AdditionalFormatterParameters parameters)
		{
			FormatterImplHelper.RunFormatterForGeneratedLanguages(originalFile, firstNode, lastNode, profile,
				it => true, PsiLanguageCategories.Dominant, parameters);
		}
	}
}
