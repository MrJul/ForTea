using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	[DaemonStage(StagesBefore = new[] { typeof(SmartResolverStage) }, StagesAfter = new[] { typeof(IdentifierHighlightingStage) })]
	public class T4CSharpHighlightingStage : CSharpDaemonStageBase {

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind, ICSharpFile file) {
			return new T4CSharpHighlightingProcess(process, settings, file);
		}

	}

}