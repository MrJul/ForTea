using System;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using System.Linq;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	[QuickFix]
	public class MoveStatementBlockQuickFix : QuickFixBase {

		[NotNull] private readonly StatementAfterFeatureHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {

			T4StatementBlock statementBlock = _highlighting.AssociatedNode;
			var file = statementBlock.GetContainingFile() as IT4File;
			Assertion.AssertNotNull(file, "file != null");

			T4FeatureBlock feature = file.GetFeatureBlocks().First();

			ITreeNode featureBlock;
			using (WriteLockCookie.Create(file.IsPhysical())) {

				// clone the statement block and add it before the feature block
				ITreeNode featurePrevSibling = feature.PrevSibling;
				featureBlock = ModificationUtil.CloneNode(statementBlock, node => { });
				featureBlock = ModificationUtil.AddChildBefore(feature, featureBlock);

				// add a new line before the new statement block if needed
				if (featurePrevSibling != null && featurePrevSibling.GetTokenType() == T4TokenNodeTypes.NewLine)
					ModificationUtil.AddChildAfter(featureBlock, T4TokenNodeTypes.NewLine.CreateLeafElement());

				ModificationUtil.DeleteChild(statementBlock);
			}

			return textControl => {
				TextRange range = featureBlock.GetDocumentRange().TextRange;
				textControl.Caret.MoveTo(range.EndOffset, CaretVisualPlacement.Generic);
			};
		}

		public override string Text
			=> "Move statement block before first class feature block";

		public override bool IsAvailable(IUserDataHolder cache)
			=> _highlighting.IsValid();

		public MoveStatementBlockQuickFix([NotNull] StatementAfterFeatureHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}
}