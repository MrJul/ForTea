using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util.Text;

namespace GammaJul.ForTea.Core.Psi.CodeStyle {

	// TODO: implement the formatter
	[Language(typeof(T4Language))]
	public class T4CodeFormatter : CodeFormatterBase<T4FormatSettingsKey> {

		protected override CodeFormattingContext CreateFormatterContext(
			CodeFormatProfile profile,
			ITreeNode firstNode,
			ITreeNode lastNode,
			AdditionalFormatterParameters parameters,
			ICustomFormatterInfoProvider provider
		)
			=> new CodeFormattingContext(this, firstNode, lastNode, profile, FormatterLoggerProvider.FormatterLogger, parameters);

		public override bool IsWhitespaceToken(ITokenNode token)
			=> token.IsWhitespaceToken();

		protected override bool IsFormatNextSpaces(CodeFormatProfile profile)
			=> false;

		public override void FormatInsertedNodes(ITreeNode nodeFirst, ITreeNode nodeLast, bool formatSurround) {
		}

		public override ITreeRange FormatInsertedRange(ITreeNode nodeFirst, ITreeNode nodeLast, ITreeRange origin)
			=> origin;

		public override void FormatReplacedNode(ITreeNode oldNode, ITreeNode newNode) {
		}

		public override void FormatReplacedRange(ITreeNode first, ITreeNode last, ITreeRange oldNodes) {
		}

		public override void FormatDeletedNodes(ITreeNode parent, ITreeNode prevNode, ITreeNode nextNode) {
		}

		public override ITokenNode GetMinimalSeparator(ITokenNode leftToken, ITokenNode rightToken)
			=> null;

		public override ITreeNode CreateSpace(string indent, ITreeNode replacedSpace)
			=> T4TokenNodeTypes.WHITE_SPACE.Create(indent);

		public override ITreeNode CreateNewLine(LineEnding lineEnding, NodeType lineBreakType = null)
			=> T4TokenNodeTypes.NEW_LINE.Create(lineEnding.GetPresentation());

		public override ITreeRange Format(
			ITreeNode firstElement,
			ITreeNode lastElement,
			CodeFormatProfile profile,
			AdditionalFormatterParameters parameters = null
		)
			=> new TreeRange(firstElement, lastElement);

		public T4CodeFormatter([NotNull] T4Language t4Language, [NotNull] CodeFormatterRequirements requirements)
			: base(t4Language, requirements) {
		}

	}

}