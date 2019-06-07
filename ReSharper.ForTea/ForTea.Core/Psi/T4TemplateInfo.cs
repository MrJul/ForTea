using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi
{
	public sealed class T4TemplateInfo
	{
		private T4TemplateInfo(IProjectFile file, ISolution solution, [NotNull] IProject project)
		{
			File = file;
			Solution = solution;
			Project = project;
		}

		[NotNull]
		public IProjectFile File { get; }

		[NotNull]
		public IProject Project { get; }

		[NotNull]
		public ISolution Solution { get; }

		public static T4TemplateInfo FromFile([NotNull] IProjectFile file)
		{
			ISolution solution = file.GetSolution();
			IProject project = file.GetProject();
			Assertion.AssertNotNull(project, "project != null");

			return new T4TemplateInfo(
				file,
				solution,
				project
			);
		}
	}
}
