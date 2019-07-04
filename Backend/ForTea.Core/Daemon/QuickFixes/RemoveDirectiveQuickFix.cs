using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes {

	[QuickFix]
	public class RemoveDirectiveQuickFix : QuickFixBase {

		[NotNull] private readonly IgnoredAssemblyDirectiveHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			ITreeNode node = _highlighting.AssociatedNode;
			using (WriteLockCookie.Create(node.IsPhysical())) {
				ITokenNode nextToken = node.GetNextToken();
				ITreeNode end = nextToken != null && nextToken.GetTokenType() == T4TokenNodeTypes.NewLine ? nextToken : node;
				ModificationUtil.DeleteChildRange(node, end);
			}
			return null;
		}

		public override string Text
			=> "Remove directive";

		public override bool IsAvailable(IUserDataHolder cache)
			=> _highlighting.IsValid();

		public RemoveDirectiveQuickFix([NotNull] IgnoredAssemblyDirectiveHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}