using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Text;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Psi.CodeStyle {

	// TODO: implement the formatter
	[Language(typeof(T4Language))]
	public class T4CodeFormatter : CodeFormatterBase {
		
		protected override PsiLanguageType LanguageType {
			get { return T4Language.Instance; }
		}

		public override bool IsWhitespaceToken(ITokenNode token) {
			return token.IsWhitespaceToken();
		}

		protected override bool IsFormatNextSpaces(CodeFormatProfile profile) {
			return false;
		}

		public override ITreeNode[] CreateSpace(string indent, ITreeNode rightNonSpace, ITreeNode replacedSpace) {
			return new ITreeNode[] {
				TreeElementFactory.CreateLeafElement(T4TokenNodeTypes.Space, new StringBuffer(indent), 0, indent.Length)
			};
		}

		public override ITreeRange Format(ITreeNode firstElement, ITreeNode lastElement, CodeFormatProfile profile, IProgressIndicator progressIndicator, IContextBoundSettingsStore overrideSettingsStore = null) {
			return new TreeRange(firstElement, lastElement);
		}

		public override void FormatInsertedNodes(ITreeNode nodeFirst, ITreeNode nodeLast, bool formatSurround) {
		}

		public override ITreeRange FormatInsertedRange(ITreeNode nodeFirst, ITreeNode nodeLast, ITreeRange origin) {
			return origin;
		}

		public override void FormatReplacedNode(ITreeNode oldNode, ITreeNode newNode) {
		}

		public override void FormatDeletedNodes(ITreeNode parent, ITreeNode prevNode, ITreeNode nextNode) {
		}

		public override ITokenNode GetMinimalSeparator(ITokenNode leftToken, ITokenNode rightToken) {
			return null;
		}

		/// <summary>
		/// Format the given subtree
		/// </summary>
		public override void Format(ITreeNode root, CodeFormatProfile profile, IProgressIndicator progressIndicator, IContextBoundSettingsStore overrideSettingsStore = null) {
			base.Format(root, profile, progressIndicator, overrideSettingsStore);
		}

		/// <summary>
		/// Format the given range in document
		/// </summary>
		public override void Format(ISolution solution, DocumentRange docRange, CodeFormatProfile profile, bool formatSpacesBefore, bool formatSpacesAfter,
			IProgressIndicator progressIndicator, IContextBoundSettingsStore overrideSettingsStore = null) {
			base.Format(solution, docRange, profile, formatSpacesBefore, formatSpacesAfter, progressIndicator, overrideSettingsStore);
		}

		public T4CodeFormatter(ISettingsStore settingsStore)
			: base(settingsStore) {
		}

	}

}