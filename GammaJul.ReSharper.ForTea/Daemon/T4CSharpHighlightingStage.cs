using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	[DaemonStage(StagesBefore = new[] { typeof(SmartResolverStage) }, StagesAfter = new[] { typeof(IdentifierHighlightingStage) })]
	public class T4CSharpHighlightingStage : CSharpDaemonStageBase {

		protected override bool IsSupported(IPsiSourceFile sourceFile) {
			return base.IsSupported(sourceFile) && sourceFile.IsLanguageSupported<T4Language>();
		}


		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind, ICSharpFile file) {
			return new T4CSharpHighlightingProcess(process, settings, file);
		}

	}

}