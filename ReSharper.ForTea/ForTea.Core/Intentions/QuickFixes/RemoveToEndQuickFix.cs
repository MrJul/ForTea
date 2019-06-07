using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Intentions.QuickFixes {

	[QuickFix]
	public class RemoveToEndQuickFix : QuickFixBase {

		[NotNull] private readonly AfterLastFeatureHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			ITreeNode startNode = _highlighting.AssociatedNode;

			var file = startNode.GetContainingFile();
			Assertion.AssertNotNull(file, "file != null");

			ITreeNode endNode = file.LastChild ?? startNode;
			using (WriteLockCookie.Create(file.IsPhysical()))
				ModificationUtil.DeleteChildRange(startNode, endNode);

			return null;
		}

		public override string Text
			=> "Remove";

		public override bool IsAvailable(IUserDataHolder cache)
			=> _highlighting.IsValid();

		public RemoveToEndQuickFix([NotNull] AfterLastFeatureHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}