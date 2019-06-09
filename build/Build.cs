using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[CheckBuildProjectConfigurations]
internal class Build : NukeBuild {

	/// Support plugins are available for:
	/// - JetBrains ReSharper        https://nuke.build/resharper
	/// - JetBrains Rider            https://nuke.build/rider
	/// - Microsoft VisualStudio     https://nuke.build/visualstudio
	/// - Microsoft VSCode           https://nuke.build/vscode

	[Parameter] public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
	[Parameter] public readonly string NuGetSource = "https://plugins.jetbrains.com/";
	[Parameter] public readonly string NuGetApiKey;

	[Solution] private readonly Solution _solution;

	private const string MainProjectName = "GammaJul.ReSharper.ForTea";

	private AbsolutePath SourceDirectory => RootDirectory / "src";
	private AbsolutePath MainProjectDirectory => SourceDirectory / MainProjectName;
	private AbsolutePath OutputDirectory => RootDirectory / "output" / "ReSharper" / Configuration;

	public Target Clean
		=> _ => _
			.Before(Restore)
			.Executes(() => {
				SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
				EnsureCleanDirectory(OutputDirectory);
			});

	public Target Restore
		=> _ => _
			.Executes(() => {
				MSBuild(s => s
					.SetToolPath("msbuild")
					.SetTargetPath(_solution)
					.SetTargets("Restore"));
			});

	public Target Compile
		=> _ => _
			.DependsOn(Restore)
			.Executes(() => {
				MSBuild(s => s
					.SetToolPath("msbuild")
					.SetTargetPath(_solution)
					.SetTargets("Rebuild")
					.SetConfiguration(Configuration)
					.SetMaxCpuCount(Environment.ProcessorCount)
					.SetNodeReuse(IsLocalBuild));
			});

	public Target Pack
		=> _ => _
			.DependsOn(Compile)
			.Executes(() => {
				var version = GetReleaseVersion();
				var currentYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
				var releaseNotes = GetReleaseNotes();
				var wave = GetWaveVersion();

				NuGetPack(s => s
					.SetTargetPath(MainProjectDirectory / (MainProjectName + ".nuspec"))
					.SetBasePath(MainProjectDirectory)
					.SetOutputDirectory(OutputDirectory)
					.SetProperty("version", version)
					.SetProperty("currentyear", currentYear)
					.SetProperty("releasenotes", releaseNotes)
					.SetProperty("wave", wave)
					.SetProperty("configuration", Configuration.ToString())
					.EnableNoPackageAnalysis());
			});

	public Target Push
		=> _ => _
			.DependsOn(Pack)
			.Requires(() => NuGetApiKey)
			.Requires(() => Configuration.Release.Equals(Configuration))
			.Executes(() => GlobFiles(OutputDirectory, "*.nupkg")
				.ForEach(x => NuGetPush(s => s
					.SetTargetPath(x)
					.SetSource(NuGetSource)
					.SetApiKey(NuGetApiKey))));

	private string GetReleaseVersion()
		=> File.ReadAllLines(MainProjectDirectory / "Properties/AssemblyInfo.cs")
			.Select(x => Regex.Match(x, @"^\[assembly: AssemblyVersion\(""([^""]*)""\)\]$"))
			.Where(x => x.Success)
			.Select(x => x.Groups[1].Value)
			.FirstOrDefault();

	private static string GetReleaseNotes()
		=> File.ReadAllLines(RootDirectory.Parent / "CHANGELOG.md")
			.SkipWhile(x => !x.StartsWith("##", StringComparison.Ordinal))
			.Skip(1)
			.TakeWhile(x => !String.IsNullOrWhiteSpace(x))
			.Select(x => $"\u2022{x.TrimStart('-')}")
			.JoinNewLine();

	private string GetWaveVersion()
		=> NuGetPackageResolver.GetLocalInstalledPackages(MainProjectDirectory / (MainProjectName + ".csproj"))
			.SingleOrDefault(x => x.Id == "Wave")
			.NotNull("fullWaveVersion != null")
			.Version
			.Version
			.ToString(2);

	public static int Main()
		=> Execute<Build>(x => x.Compile);

}