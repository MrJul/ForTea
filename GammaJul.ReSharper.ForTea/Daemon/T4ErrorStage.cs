using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Daemon stage that creates processes for adding error and warning highlights.
	/// </summary>
	[DaemonStage(StagesBefore = new[] { typeof(T4HighlightingStage) })]
	public class T4ErrorStage : T4DaemonStage {

		private readonly DirectiveInfoManager _directiveInfoManager;

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file) {
			return new T4ErrorProcess(file, process, _directiveInfoManager);
		}

		/// <summary>
		/// Check the error stripe indicator necessity for this stage after processing given <paramref name="sourceFile"/>
		/// </summary>
		public override ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore) {
			return ErrorStripeRequest.STRIPE_AND_ERRORS;
		}

		public T4ErrorStage([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}
}