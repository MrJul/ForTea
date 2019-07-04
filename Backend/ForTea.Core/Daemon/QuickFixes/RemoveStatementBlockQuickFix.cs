using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes {

	[QuickFix]
	public class RemoveStatementBlockQuickFix : QuickFixBase {

		[NotNull] private readonly StatementAfterFeatureHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			using (WriteLockCookie.Create(_highlighting.AssociatedNode.IsPhysical()))
				ModificationUtil.DeleteChild(_highlighting.AssociatedNode);

			return null;
		}

		public override string Text
			=> "Remove statement block";

		public override bool IsAvailable(IUserDataHolder cache)
			=> _highlighting.IsValid();

		public RemoveStatementBlockQuickFix([NotNull] StatementAfterFeatureHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}