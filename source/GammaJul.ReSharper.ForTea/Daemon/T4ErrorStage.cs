using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>Daemon stage that creates processes for adding error and warning highlights.</summary>
	[DaemonStage(StagesBefore = new[] { typeof(T4HighlightingStage), typeof(CollectUsagesStage) })]
	public class T4ErrorStage : T4DaemonStage {

		[NotNull] private readonly DirectiveInfoManager _directiveInfoManager;

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file)
			=> new T4ErrorProcess(file, process, _directiveInfoManager);

		public T4ErrorStage([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}
}