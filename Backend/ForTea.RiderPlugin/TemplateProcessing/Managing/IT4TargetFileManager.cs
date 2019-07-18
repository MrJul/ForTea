using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TargetFileManager
	{
		[NotNull]
		FileSystemPath SaveResults(string result, [NotNull] IT4File file, [CanBeNull] string targetExtension = null);
	}
}
