using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Daemon stage that creates processes for highlighting tokens.
	/// </summary>
	[DaemonStage]
	public class T4HighlightingStage : T4DaemonStage {

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file) {
			return new T4HighlightingProcess(file, process);
		}

		/// <summary>
		/// Check the error stripe indicator necessity for this stage after processing given <paramref name="sourceFile"/>
		/// </summary>
		public override ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore) {
			return ErrorStripeRequest.NONE;
		}

	}

}