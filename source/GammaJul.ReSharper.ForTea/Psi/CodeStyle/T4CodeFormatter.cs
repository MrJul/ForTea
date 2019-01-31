#region License

//    Copyright 2012 Julien Lebosquain
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


using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util.Text;

namespace GammaJul.ReSharper.ForTea.Psi.CodeStyle {

	// TODO: implement the formatter
	[Language(typeof(T4Language))]
	public class T4CodeFormatter : CodeFormatterBase<T4FormatSettingsKey> {

		public override PsiLanguageType LanguageType
			=> T4Language.Instance;

		protected override CodeFormattingContext CreateFormatterContext(
			CodeFormatProfile profile,
			ITreeNode firstNode,
			ITreeNode lastNode,
			AdditionalFormatterParameters parameters,
			ICustomFormatterInfoProvider provider)
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
			=> T4TokenNodeTypes.Space.Create(indent);

		public override ITreeNode CreateNewLine(LineEnding lineEnding, NodeType lineBreakType = null)
			=> T4TokenNodeTypes.NewLine.Create(lineEnding.GetPresentation());


		public override ITreeRange Format(
			ITreeNode firstElement,
			ITreeNode lastElement,
			CodeFormatProfile profile,
			AdditionalFormatterParameters parameters = null)
			=> new TreeRange(firstElement, lastElement);

		public T4CodeFormatter([NotNull] CodeFormatterRequirements requirements)
			: base(requirements) {
		}

	}

}