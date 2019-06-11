using GammaJul.ReSharper.ForTea.Common;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using Microsoft.VisualStudio.Shell.Interop;

namespace GammaJul.ReSharper.ForTea.VisualStudio
{
	internal static class Utils
	{
		[CanBeNull]
		public static IVsHierarchy TryGetVsHierarchy([NotNull] ProjectInfo info) => info
			.Solution
			.TryGetComponent<ProjectModelSynchronizer>()
			?.TryGetHierarchyItemByProjectItem(info.Project, false)
			?.Hierarchy;

	}
}
