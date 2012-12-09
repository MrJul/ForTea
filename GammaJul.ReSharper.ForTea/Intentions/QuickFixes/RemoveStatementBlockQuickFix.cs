using System;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	[QuickFix]
	public class RemoveStatementBlockQuickFix : QuickFixBase {
		private readonly StatementAfterFeatureHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			using (_highlighting.AssociatedNode.CreateWriteLock())
				ModificationUtil.DeleteChild(_highlighting.AssociatedNode);
			return null;
		}

		public override string Text {
			get { return "Remove statement block"; }
		}

		public override bool IsAvailable(IUserDataHolder cache) {
			return _highlighting.IsValid();
		}

		public RemoveStatementBlockQuickFix([NotNull] StatementAfterFeatureHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}