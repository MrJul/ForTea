using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
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
			var project = file.GetProject().NotNull();
			return new Dictionary<string, string>
			{
				{"Configuration", project.ProjectProperties.ActiveConfigurations.Configurations.Single().Name},
//			{"DevEnvDir", null},
//			{"FrameworkDir", null},
//			{"FrameworkSDKDir", null},
//			{"FrameworkVersion", null},
//			{"FxCopDir", null},
//			{"IntDir", null},
				{"OutDir", project.GetOutputFilePath(project.GetCurrentTargetFrameworkId()).Parent.FullPath},
//			{"Platform", null},
//			{"PlatformShortName", null},
				{"ProjectDir", project.Location.Parent.FullPath},
//			{"ProjectExt", null},
				{"ProjectFileName", project.ProjectFileLocation.Name},
				{"ProjectName", project.Name},
				{"ProjectPath", project.Location.FullPath},
//			{"RemoteMachine", null},
				{"RootNameSpace", project.GetUniqueRequestedProjectProperty(MSBuildProjectUtil.RootNamespaceProperty)},
				{"SolutionDir", Solution.SolutionDirectory.FullPath},
//			{"SolutionExt", null},
//			{"SolutionFileName", null},
				{"SolutionName", Solution.Name},
				{"SolutionPath", Solution.SolutionFilePath.FullPath},
//			{"TargetDir", null},
				{"TargetExt", T4MSBuildProjectUtil.GetTargetExtension(project)},
//			{"TargetFileName", null},
//			{"TargetName", null},
//			{"TargetPath", null},
//			{"VCInstallDir", null},
//			{"VSInstallDir", null},
//			{"WebDeployPath", null},
//			{"WebDeployRoot", null}
			};
		}

		public T4MacroResolver([NotNull] ISolution solution, [NotNull] IT4AssemblyNamePreprocessor preprocessor) :
			base(preprocessor) => Solution = solution;
	}
}
