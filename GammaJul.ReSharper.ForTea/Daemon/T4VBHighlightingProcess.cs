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


using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.VB.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB.Parsing;
using JetBrains.ReSharper.Psi.VB.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Highlights VB keywords, identifiers, etc.
	/// Usually, those are colored by Visual Studio language service. However, it's not available in T4 files so we have to do it ourselves.
	/// </summary>
	internal class T4VBHighlightingProcess : VBIncrementalDaemonStageProcess {

		private static readonly NodeTypeSet _vbOperators = new NodeTypeSet(
			VBTokenType.COMMA,
			VBTokenType.DOT,
			VBTokenType.EQ,
			VBTokenType.GT,
			VBTokenType.GE,
			VBTokenType.LT,
			VBTokenType.LE,
			VBTokenType.NE,
			VBTokenType.EXCL,
			VBTokenType.COLON,
			VBTokenType.PLUS,
			VBTokenType.PLUSEQ,
			VBTokenType.MINUS,
			VBTokenType.MINUSEQ,
			VBTokenType.ASTERISK,
			VBTokenType.ASTERISKEQ,
			VBTokenType.DIV,
			VBTokenType.DIVEQ,
			VBTokenType.AND,
			VBTokenType.ANDEQ,
			VBTokenType.XOR,
			VBTokenType.XOREQ,
			VBTokenType.LTLT,
			VBTokenType.GTGT,
			VBTokenType.LTLTEQ,
			VBTokenType.GTGTEQ,
			VBTokenType.QUESTION,
			VBTokenType.TYPECHAR_PERC,
			VBTokenType.TYPECHAR_AND,
			VBTokenType.TYPECHAR_EXCL
			);

		public T4VBHighlightingProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, IVBFile file)
			: base(process, settingsStore, file) {
		}

		public override void VisitNode(ITreeNode node, IHighlightingConsumer context) {
			base.VisitNode(node, context);

			var highlightingRange = node.GetHighlightingRange();
			context.AddHighlighting(new PredefinedHighlighting(VsPredefinedHighlighterIds.RazorCode, highlightingRange));
			bool xml;
			var attributeId = GetHighlightingAttributeId(node, out xml);

			if (attributeId != null)
				context.AddHighlighting(new PredefinedHighlighting(attributeId, highlightingRange));

			// Dim embedded xml literals if desired...
			//if (xml)
			//    context.AddHighlighting(new PredefinedHighlighting(HighlightingAttributeIds.DEADCODE_ATTRIBUTE, highlightingRange));
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element, out bool isXml) {
			isXml = false;
			var tokenType = element.GetTokenType();
			if (tokenType == null)
				return null;

			// XML LITERALS
			isXml = true;
			if (tokenType == VBTokenType.XML_SCRIPLET_START || tokenType == VBTokenType.XML_SCRIPLET_END)
				return VsPredefinedHighlighterIds.HtmlServerSideScript;
			if (tokenType is XmlTokenNodeType)
				return GetXmlHighlightingAttributeId(element, tokenType);
			isXml = false;

			// VB.NET LANGUAGE
			if (tokenType.IsKeyword)
				return VsPredefinedHighlighterIds.Keyword;
			if (tokenType.IsComment)
				return VsPredefinedHighlighterIds.Comment;
			if (tokenType.IsStringLiteral)
				return VsPredefinedHighlighterIds.String;

			if (tokenType.IsConstantLiteral) {
				if (tokenType == VBTokenType.CHAR_LITERAL)
					return VsPredefinedHighlighterIds.String;

				return VsPredefinedHighlighterIds.Number;
			}

			if (_vbOperators[tokenType])
				return VsPredefinedHighlighterIds.Operator;

			if (tokenType.IsIdentifier) {
				var vbidentifier = element.Parent as IVBIdentifier;
				if (vbidentifier != null) {
					ResolveResultWithInfo reference = null;

					var referenceName = vbidentifier.Parent as IReferenceName;
					if (referenceName != null)
						reference = referenceName.Reference.Resolve();

					var referenceExpresion = vbidentifier.Parent as IReferenceExpression;
					if (referenceExpresion != null)
						reference = referenceExpresion.Reference.Resolve();

					if (reference != null) {
						var typeElement = reference.DeclaredElement as ITypeElement;
						if (typeElement != null)
							return HighlightingAttributeIds.GetHighlightAttributeForTypeElement(typeElement);
					}
				}

				return VsPredefinedHighlighterIds.Identifier;
			}

			return null;
		}

		[CanBeNull]
		private static string GetXmlHighlightingAttributeId(ITreeNode element, TokenNodeType token) {
			// only highlight the first identifier in an xml tag (the rest are attributes)
			if (token.IsIdentifier) {
				var prevSibling = element.PrevSibling;
				if (prevSibling != null) {
					var prevSiblingToken = prevSibling.GetTokenType();
					if (prevSiblingToken != null) {
						if (prevSiblingToken == VBTokenType.XmlTokens.TAG_START ||
							prevSiblingToken == VBTokenType.XmlTokens.TAG_START1)
							return VsPredefinedHighlighterIds.HtmlElementName;
					}
				}

				return VsPredefinedHighlighterIds.HtmlAttributeName;
			}

			if (token == VBTokenType.XmlTokens.TAG_START ||
				token == VBTokenType.XmlTokens.TAG_END ||
				token == VBTokenType.XmlTokens.TAG_START1 ||
				token == VBTokenType.XmlTokens.TAG_END1 ||
				token == VBTokenType.XmlTokens.EQ)
				return HighlightingAttributeIds.REGEXP_COMMENT;

			if (token == VBTokenType.XmlTokens.COMMENT_START ||
				token == VBTokenType.XmlTokens.COMMENT_END ||
				token == VBTokenType.XmlTokens.COMMENT_BODY)
				return HighlightingAttributeIds.REGEXP_COMMENT;

			return null;
		}

	}

}