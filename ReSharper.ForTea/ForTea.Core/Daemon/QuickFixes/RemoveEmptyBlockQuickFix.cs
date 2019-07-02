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

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
	[QuickFix]
	public class RemoveEmptyBlockQuickFix : QuickFixBase
	{
		[NotNull]
		private EmptyBlockHighlighting Highlighting { get; }

		public RemoveEmptyBlockQuickFix([NotNull] EmptyBlockHighlighting highlighting) => Highlighting = highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			ITreeNode node = Highlighting.AssociatedNode;
			using (WriteLockCookie.Create(node.IsPhysical()))
			{
				var nextToken = node.GetNextToken();
				ITreeNode end;
				if (nextToken != null && nextToken.GetTokenType() == T4TokenNodeTypes.NewLine) end = nextToken;
				else end = node;
				ModificationUtil.DeleteChildRange(node, end);
			}

			return null;
		}

		public override string Text => "Remove empty block";
		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
	}
}
