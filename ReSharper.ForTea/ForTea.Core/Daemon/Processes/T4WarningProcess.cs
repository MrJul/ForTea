using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	public class T4WarningProcess : T4DaemonStageProcess
	{
		public T4WarningProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess) :
			base(file, daemonProcess)
		{
		}

		public override void ProcessAfterInterior(ITreeNode element)
		{
			if (element is T4FeatureBlock) return;
			if (!(element is T4CodeBlock block)) return;
			if (!block.GetCodeText().IsNullOrWhitespace()) return;
			AddHighlighting(element.GetDocumentRange(), new EmptyBlockHighlighting(block));
		}
	}
}
