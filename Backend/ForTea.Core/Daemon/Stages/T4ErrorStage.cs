using GammaJul.ForTea.Core.Daemon.Processes;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Stages {

	/// <summary>Daemon stage that creates processes for adding error and warning highlights.</summary>
	[DaemonStage(StagesBefore = new[] { typeof(CollectUsagesStage) })]
	public class T4ErrorStage : T4DaemonStageBase {

		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file)
			=> new T4ErrorProcess(file, process, _directiveInfoManager);

		public T4ErrorStage([NotNull] T4DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}
}
