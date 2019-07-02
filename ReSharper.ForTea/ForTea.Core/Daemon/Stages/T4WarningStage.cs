using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
	[DaemonStage(StagesBefore = new[] {typeof(T4ErrorStage)})]
	public class T4WarningStage : T4DaemonStage
	{
		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file) =>
			new T4WarningProcess(file, process);
	}
}
