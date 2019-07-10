using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TargetFileManager
	{
		[NotNull]
		IProjectFile GetOrCreateDestinationFile(
			[NotNull] IT4File file,
			[NotNull] IProjectModelTransactionCookie cookie,
			[CanBeNull] string targetExtension = null
		);

		[NotNull]
		FileSystemPath GetDestinationPath([NotNull] IT4File file, [CanBeNull] string targetExtension = null);
	}
}
