using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;

namespace GammaJul.ReSharper.ForTea
{
	public sealed class ProjectInfo
	{
		public ProjectInfo(IProjectFile projectFile, ISolution solution, [NotNull] IProject project)
		{
			ProjectFile = projectFile;
			Solution = solution;
			Project = project;
		}

		[NotNull]
		public IProjectFile ProjectFile { get; }

		[NotNull]
		public IProject Project { get; }

		[NotNull]
		public ISolution Solution { get; }

		public static ProjectInfo FromFile([NotNull] IProjectFile file)
		{
			ISolution solution = file.GetSolution();
			IProject project = file.GetProject();
			Assertion.AssertNotNull(project, "project != null");

			return new ProjectInfo(
				file,
				solution,
				project
			);
		}
	}
}
