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
using System;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	/// <summary>
	/// Quick fix that add a missing token (usually block tags).
	/// </summary>
	[QuickFix]
	public class AddMissingTokenQuickFix : QuickFixBase {
		private readonly MissingTokenHighlighting _highlighting;

		/// <summary>
		/// Executes QuickFix or ContextAction. Returns post-execute method.
		/// </summary>
		/// <returns>
		/// Action to execute after document and PSI transaction finish. Use to open TextControls, navigate caret, etc.
		/// </returns>
		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			MissingTokenErrorElement errorElement = _highlighting.AssociatedNode;
			IFile file = errorElement.GetContainingFile();
			Assertion.AssertNotNull(file, "file != null");

			TreeTextRange modifiedRange;
			TextRange hotspotRange;

			// replace the error token by the missing text
			using (file.CreateWriteLock()) {
				
				modifiedRange = errorElement.GetTreeTextRange();
				ITokenNode previousToken = errorElement.GetPreviousToken();
				if (previousToken != null && previousToken.GetTokenType() == T4TokenNodeTypes.NewLine)
					modifiedRange = modifiedRange.Join(previousToken.GetTreeTextRange());
				Assertion.AssertNotNull(file, "file != null");

				Pair<string, TextRange> textWithHotspotRange = GetMissingTextWithHotspotRange(errorElement);
				hotspotRange = textWithHotspotRange.Second;

				file.ReParse(modifiedRange, textWithHotspotRange.First);
			}

			if (!hotspotRange.IsValid)
				return null;

			// create a hotspot around a directive name or value, with basic completion invoked
			return textControl => {

				DocumentRange initialRange = file.GetDocumentRange(modifiedRange.StartOffset);
				int startOffset = initialRange.TextRange.StartOffset;
				var finalRange = new TextRange(startOffset + hotspotRange.StartOffset, startOffset + hotspotRange.EndOffset);
				string fieldName = textControl.Document.GetText(finalRange);

				Shell.Instance
					.GetComponent<LiveTemplatesManager>()
					.CreateHotspotSessionAtopExistingText(
						solution,
						TextRange.InvalidRange,
						textControl,
						LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
						HotspotHelper.CreateBasicCompletionHotspotInfo(fieldName, new DocumentRange(initialRange.Document, finalRange)))
					.Execute();
			};
		}

		private struct TextInfo {
			internal readonly string Text;
			internal readonly bool SpaceBefore;
			internal readonly bool SpaceAfter;
			internal readonly TextRange HotspotRange;

			internal TextInfo(string text, bool spaceBefore, bool spaceAfter, TextRange hotspotRange) {
				Text = text;
				SpaceBefore = spaceBefore;
				SpaceAfter = spaceAfter;
				HotspotRange = hotspotRange;
			}
		}

		/// <summary>
		/// Gets the text of the token to add.
		/// </summary>
		/// <param name="missingTokenType">The associated missing token type.</param>
		/// <returns>A <see cref="TextInfo"/> containing the text to add with spacing.</returns>
		private static TextInfo GetMissingTextInfo(MissingTokenType missingTokenType) {
			switch (missingTokenType) {
				case MissingTokenType.BlockEnd: return new TextInfo("#>", true, false, TextRange.InvalidRange);
				case MissingTokenType.DirectiveName: return new TextInfo("name", true, true, new TextRange(0, 4));
				case MissingTokenType.AttributeName: return new TextInfo("name", true, false, new TextRange(0, 4));
				case MissingTokenType.AttributeNameAndEqualSign: return new TextInfo("name=", true, false, new TextRange(0, 4));
				case MissingTokenType.EqualSign: return new TextInfo("=", false, false, TextRange.InvalidRange);
				case MissingTokenType.AttributeValue: return new TextInfo("\"value\"", false, true, new TextRange(1, 6));
				case MissingTokenType.EqualSignAndAttributeValue: return new TextInfo("=\"value\"", false, true, new TextRange(2, 7));
				case MissingTokenType.Quote: return new TextInfo("\"", false, true, TextRange.InvalidRange);
				default: throw new ArgumentOutOfRangeException("missingTokenType");
			}
		}

		private static bool IsWhitespaceBefore([NotNull] ITreeNode node) {
			ITokenNode previousToken = node.GetPreviousToken();
			return previousToken != null
				&& (previousToken.IsWhitespaceToken() || Char.IsWhiteSpace(previousToken.GetText().Last()));
		}

		private static bool IsWhitespaceAfter([NotNull] ITreeNode node) {
			ITokenNode nextToken = node.GetNextToken();
			return nextToken != null
				&& (nextToken.IsWhitespaceToken() || Char.IsWhiteSpace(nextToken.GetText()[0]));
		}

		/// <summary>
		/// Gets the text of the token to add, with additional spaces if needed.
		/// </summary>
		/// <param name="errorElement">The associated error element.</param>
		/// <returns>A pair containing the representation of <paramref name="errorElement"/> and its hotspot range.</returns>
		private static Pair<string, TextRange> GetMissingTextWithHotspotRange([NotNull] MissingTokenErrorElement errorElement) {
			TextInfo textInfo = GetMissingTextInfo(errorElement.MissingTokenType);
			TextRange hotSpotRange = textInfo.HotspotRange;

			string text = textInfo.Text;
			if (textInfo.SpaceBefore && !IsWhitespaceBefore(errorElement)) {
				text = " " + text;
				if (hotSpotRange.IsValid)
					hotSpotRange = hotSpotRange.Shift(1);
			}
			if (textInfo.SpaceAfter && !IsWhitespaceAfter(errorElement))
				text += " ";

			return Pair.Of(text, hotSpotRange);
		}
		
		private static string GetFixText(MissingTokenType missingTokenType) {
			switch (missingTokenType) {
				case MissingTokenType.BlockEnd: return "Add block end";
				case MissingTokenType.DirectiveName: return "Add directive name";
				case MissingTokenType.AttributeName: return "Add attribute name";
				case MissingTokenType.AttributeNameAndEqualSign: return "Add attribute name and equal sign";
				case MissingTokenType.EqualSign: return "Add equal sign";
				case MissingTokenType.AttributeValue: return "Add attribute value";
				case MissingTokenType.EqualSignAndAttributeValue: return "Add equal sign and attribute value";
				case MissingTokenType.Quote: return "Add quote";
				default: throw new ArgumentOutOfRangeException("missingTokenType");
			}
		}

		/// <summary>
		/// Popup menu item text
		/// </summary>
		public override string Text {
			get { return GetFixText(_highlighting.AssociatedNode.MissingTokenType); }
		}

		/// <summary>
		/// Check if this action is available at the constructed context.
		/// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
		/// </summary>
		public override bool IsAvailable(IUserDataHolder cache) {
			return _highlighting.IsValid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AddMissingTokenQuickFix"/> class.
		/// </summary>
		/// <param name="highlighting">The associated missing token highlighting.</param>
		public AddMissingTokenQuickFix(MissingTokenHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}