using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Stages
{
	/// <summary>Base class for every T4 daemon stage.</summary>
	public abstract class T4DaemonStageBase : IDaemonStage
	{
		public IEnumerable<IDaemonStageProcess> CreateProcess(
			IDaemonProcess process,
			IContextBoundSettingsStore settings,
			DaemonProcessKind processKind
		)
			=> process.SourceFile.GetTheOnlyPsiFile(T4Language.Instance) is IT4File t4File
				? new[] {CreateProcess(process, t4File)}
				: EmptyList<IDaemonStageProcess>.InstanceList;

		[NotNull]
		protected abstract IDaemonStageProcess CreateProcess([NotNull] IDaemonProcess process, [NotNull] IT4File file);
	}
}
