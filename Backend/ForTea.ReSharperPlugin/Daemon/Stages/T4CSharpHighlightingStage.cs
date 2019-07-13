using GammaJul.ForTea.Core.Daemon.Stages;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Application.Settings;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Processes;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Stages {

	// GlobalFileStructureCollectorStage is required before this stage (otherwise there will be an exception in CSharpIncrementalDaemonStageProcessBase).
	// CollectUsagesStage must come after this stage if we want the highlightings to appear as fast as possible.
	[DaemonStage(
		StagesBefore = new[] { typeof(GlobalFileStructureCollectorStage) },
		StagesAfter = new[] { typeof(T4CSharpErrorStage), typeof(CollectUsagesStage), typeof(IdentifierHighlightingStage)}
	)]
	public class T4CSharpHighlightingStage : CSharpDaemonStageBase {

		protected override bool IsSupported(IPsiSourceFile sourceFile)
			=> base.IsSupported(sourceFile) && sourceFile != null && sourceFile.IsLanguageSupported<T4Language>();

		protected override IDaemonStageProcess CreateProcess(
			IDaemonProcess process,
			IContextBoundSettingsStore settings,
			DaemonProcessKind processKind,
			ICSharpFile file
		)
			=> new T4CSharpHighlightingProcess(process, settings, file);

	}

}
