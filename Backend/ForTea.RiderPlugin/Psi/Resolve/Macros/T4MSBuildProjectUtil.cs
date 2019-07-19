using System.Collections.Generic;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.MSBuild;
using JetBrains.ProjectModel.Properties;

namespace JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros
{
	public static class T4MSBuildProjectUtil
	{
		public const string AutoGenElement = "AutoGen";
		public const string DesignTimeElement = "DesignTime";
		public const string True = "True";

		public static FileCreationParameters CreateTemplateMetadata(IProjectFile projectFile) =>
			new FileCreationParameters(new Dictionary<string, string>
			{
				{AutoGenElement, True},
				{DesignTimeElement, True},
				{MSBuildProjectUtil.DependentUponElement, projectFile.Name}
			});

		public static string GetTargetExtension(IProject project)
		{
			string outputType = project.GetUniqueRequestedProjectProperty(MSBuildProjectUtil.OutputTypeProperty);
			switch (outputType)
			{
				case MSBuildProjectUtil.OutputTypeExe: return ".exe";
				case MSBuildProjectUtil.OutputTypeLibrary: return ".dll";
				case MSBuildProjectUtil.OutputTypeDynamicLibrary: return ".dll";
				case MSBuildProjectUtil.OutputTypeWinExe: return ".exe";
				case MSBuildProjectUtil.OutputTypeWinMd: return ".winmdobj";
				case MSBuildProjectUtil.OutputTypeAppContainerExe: return ".exe";
				case MSBuildProjectUtil.OutputTypeModule: return ".netmodule";
			}
		}
	}
}
