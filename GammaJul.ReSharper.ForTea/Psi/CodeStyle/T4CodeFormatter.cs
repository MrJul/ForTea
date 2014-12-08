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
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

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

		public override ITreeNode CreateSpace(string indent, ITreeNode replacedSpace) {
			return T4TokenNodeTypes.Space.Create(indent);
		}

		public override ITreeNode CreateNewLine() {
			T4TokenNodeType nodeType = T4TokenNodeTypes.NewLine;
			return nodeType.Create(nodeType.TokenRepresentation);
		}

		public T4CodeFormatter(ISettingsStore settingsStore)
			: base(settingsStore) {
		}

	}

}