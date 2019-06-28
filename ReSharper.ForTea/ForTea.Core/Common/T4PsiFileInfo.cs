using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Common
{
	public class T4PsiFileInfo
	{
		[NotNull]
		public IPsiSourceFile PsiSourceFile { get; }
		
		[NotNull]
		private T4ProjectFileInfo ProjectFileInfo { get; }

		[NotNull]
		public ISolution Solution => ProjectFileInfo.Solution;

		[NotNull]
		public IProjectFile ProjectFile => ProjectFileInfo.File;

		private T4PsiFileInfo([NotNull] IPsiSourceFile psiSourceFile, [NotNull] T4ProjectFileInfo projectFileInfo)
		{
			PsiSourceFile = psiSourceFile;
			ProjectFileInfo = projectFileInfo;
		}

		public static T4PsiFileInfo FromFile([NotNull] IPsiSourceFile file)
		{
			var projectFile = file.ToProjectFile();
			Assertion.AssertNotNull(projectFile, "projectFile != null");
			var projectFileInfo = T4ProjectFileInfo.FromFile(projectFile);
			return new T4PsiFileInfo(file, projectFileInfo);
		}
	}
}
