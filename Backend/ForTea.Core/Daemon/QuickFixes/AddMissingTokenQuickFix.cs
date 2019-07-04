using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes {

	/// <summary>Quick fix that add a missing token (usually block tags).</summary>
	[QuickFix]
	public class AddMissingTokenQuickFix : QuickFixBase {

		[NotNull] private readonly MissingTokenHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {

			MissingTokenErrorElement errorElement = _highlighting.AssociatedNode;
			IFile file = errorElement.GetContainingFile();
			Assertion.AssertNotNull(file, "file != null");

			TreeTextRange modifiedRange;
			TextRange hotspotRange;

			// replace the error token by the missing text
			using (WriteLockCookie.Create(file.IsPhysical())) {

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

				DocumentRange initialRange = file.GetDocumentRange(modifiedRange);
				int startOffset = initialRange.TextRange.StartOffset;
				var finalRange = new TextRange(startOffset + hotspotRange.StartOffset, startOffset + hotspotRange.EndOffset);
				string fieldName = textControl.Document.GetText(finalRange);

				Shell.Instance
					.GetComponent<LiveTemplatesManager>()
					.CreateHotspotSessionAtopExistingText(
						solution,
						DocumentOffset.InvalidOffset, 
						textControl,
						LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
						HotspotHelper.CreateBasicCompletionHotspotInfo(fieldName, new DocumentRange(initialRange.Document, finalRange)))
					.Execute();
			};
		}

		private readonly struct TextInfo {

			[NotNull] public readonly string Text;
			public readonly bool SpaceBefore;
			public readonly bool SpaceAfter;
			public readonly TextRange HotspotRange;

			internal TextInfo([NotNull] string text, bool spaceBefore, bool spaceAfter, TextRange hotspotRange) {
				Text = text;
				SpaceBefore = spaceBefore;
				SpaceAfter = spaceAfter;
				HotspotRange = hotspotRange;
			}
		}

		/// <summary>Gets the text of the token to add.</summary>
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
				default: throw new ArgumentOutOfRangeException(nameof(missingTokenType));
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

		/// <summary>Gets the text of the token to add, with additional spaces if needed.</summary>
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
		
		[NotNull]
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
				default: throw new ArgumentOutOfRangeException(nameof(missingTokenType));
			}
		}

		public override string Text
			=> GetFixText(_highlighting.AssociatedNode.MissingTokenType);

		public override bool IsAvailable(IUserDataHolder cache)
			=> _highlighting.IsValid();

		/// <summary>Initializes a new instance of the <see cref="AddMissingTokenQuickFix"/> class.</summary>
		/// <param name="highlighting">The associated missing token highlighting.</param>
		public AddMissingTokenQuickFix(MissingTokenHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}
