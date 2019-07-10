using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.ReSharperPlugin
{
	internal static class Utils
	{
		[CanBeNull]
		public static IVsHierarchy TryGetVsHierarchy([NotNull] IProjectFile file) => file
			.GetSolution()
			.TryGetComponent<ProjectModelSynchronizer>()
			?.TryGetHierarchyItemByProjectItem(file.GetProject().NotNull(), false)
			?.Hierarchy;

	}
}
