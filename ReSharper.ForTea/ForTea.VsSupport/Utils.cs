using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.VsSupport
{
	internal static class Utils
	{
		[CanBeNull]
		public static IVsHierarchy TryGetVsHierarchy([NotNull] T4TemplateInfo info) => info
			.Solution
			.TryGetComponent<ProjectModelSynchronizer>()
			?.TryGetHierarchyItemByProjectItem(info.Project, false)
			?.Hierarchy;

	}
}
