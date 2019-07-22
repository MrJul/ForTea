using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.MSBuild;
using JetBrains.ProjectModel.Properties;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros
{
	[SolutionComponent]
	public sealed class T4MacroResolver : T4MacroResolverBase
	{
		[NotNull]
		private ISolution Solution { get; }

		public override IReadOnlyDictionary<string, string> Resolve(
			IEnumerable<string> macros,
			IProjectFile file
		)
		{
			var result = new Dictionary<string, string>
			{
//			{"DevEnvDir", null},
//			{"FrameworkDir", null},
//			{"FrameworkSDKDir", null},
//			{"FrameworkVersion", null},
//			{"FxCopDir", null},
//			{"IntDir", null},
//			{"Platform", null},
//			{"PlatformShortName", null},
//			{"ProjectExt", null},
//			{"RemoteMachine", null},
				{"SolutionDir", Solution.SolutionDirectory.FullPath},
//			{"SolutionExt", null},
				{"SolutionName", Solution.Name},
				{"SolutionPath", Solution.SolutionFilePath.FullPath},
//			{"TargetDir", null},
//			{"TargetFileName", null},
//			{"TargetName", null},
//			{"TargetPath", null},
//			{"VCInstallDir", null},
//			{"VSInstallDir", null},
//			{"WebDeployPath", null},
//			{"WebDeployRoot", null}
			};
			var solutionFile = Solution.SolutionFile;
			if (solutionFile != null) result.Add("SolutionFileName", solutionFile.Name);
			var project = file.GetProject();
			if (project == null) return result;

			result.Add("Configuration", project.ProjectProperties.ActiveConfigurations.Configurations.Single().Name);
			result.Add("OutDir", project.GetOutputFilePath(project.GetCurrentTargetFrameworkId()).Parent.FullPath);
			result.Add("ProjectDir", project.Location.Parent.FullPath);
			result.Add("ProjectFileName", project.ProjectFileLocation.Name);
			result.Add("ProjectName", project.Name);
			result.Add("ProjectPath", project.Location.FullPath);
			result.Add("RootNameSpace",
				project.GetUniqueRequestedProjectProperty(MSBuildProjectUtil.RootNamespaceProperty));
			result.Add("TargetExt", T4MSBuildProjectUtil.GetTargetExtension(project));
			return result;
		}

		public T4MacroResolver([NotNull] ISolution solution, [NotNull] IT4AssemblyNamePreprocessor preprocessor) :
			base(preprocessor) => Solution = solution;
	}
}
