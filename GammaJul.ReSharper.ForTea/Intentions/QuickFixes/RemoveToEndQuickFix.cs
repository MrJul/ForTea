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
	public class RemoveToEndQuickFix : QuickFixBase {
		private readonly AfterLastFeatureHighlighting _highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			ITreeNode startNode = _highlighting.AssociatedNode;

			var file = startNode.GetContainingFile();
			Assertion.AssertNotNull(file, "file != null");

			ITreeNode endNode = file.LastChild ?? startNode;
			using (file.CreateWriteLock())
				ModificationUtil.DeleteChildRange(startNode, endNode);

			return null;
		}

		public override string Text {
			get { return "Remove"; }
		}

		public override bool IsAvailable(IUserDataHolder cache) {
			return _highlighting.IsValid();
		}

		public RemoveToEndQuickFix([NotNull] AfterLastFeatureHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}