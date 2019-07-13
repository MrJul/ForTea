using GammaJul.ForTea.Core.Daemon.Stages;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Processes;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Stages {

	/// <summary>Daemon stage that creates processes for highlighting tokens.</summary>
	[DaemonStage(
		StagesBefore = new[] { typeof(GlobalFileStructureCollectorStage) },
		StagesAfter = new[] { typeof(T4ErrorStage), typeof(CollectUsagesStage), typeof(IdentifierHighlightingStage) }
	)]
	public class T4HighlightingStage : T4DaemonStageBase {

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file)
			=> new T4HighlightingProcess(file, process);

	}

}
