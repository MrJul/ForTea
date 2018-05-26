using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.HttpTasks;
using static Nuke.Common.IO.SerializationTasks;
using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Common.Tooling.NuGetPackageResolver;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;
using static Nuke.Common.Tooling.ProcessTasks;

class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Pack);

    [Parameter] readonly string Source = "https://resharper-plugins.jetbrains.com/api/v2/package";
    [Parameter] readonly string ApiKey;

    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    string ProjectFile => GlobFiles(SourceDirectory, "**/*.csproj").Single();

    Target InstallHive => _ => _
            .Executes(() =>
            {
                var jsonResponse = HttpDownloadString("https://data.services.jetbrains.com/products/releases?code=RSU&latest=true");
                var downloadUrl = JsonDeserialize<JObject>(jsonResponse)["RSU"].First["downloads"]["windows"]["link"].ToString();
                var installer = TemporaryDirectory / new Uri(downloadUrl).Segments.Last();
                var installationHive = MSBuildParseProject(ProjectFile).Properties["InstallationHive"];

                if (!File.Exists(installer))
                    HttpDownloadFile(downloadUrl, installer);

                Info($"Installing '{Path.GetFileNameWithoutExtension(installer)}' into '{installationHive}' hive...");
                StartProcess(installer, $"/VsVersion=12.0;14.0;15.0 /SpecificProductNames=ReSharper /Hive={installationHive} /Silent=True")
                        .AssertZeroExitCode();
            });

    Target Clean => _ => _
            .Executes(() =>
            {
                DeleteDirectories(GlobDirectories(Path.GetDirectoryName(ProjectFile), "**/bin", "**/obj"));
                EnsureCleanDirectory(OutputDirectory);
            });

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                MSBuild(s => DefaultMSBuildRestore);
            });

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                MSBuild(s => DefaultMSBuildCompile);
            });

    Target Pack => _ => _
            .DependsOn(Compile)
            .Executes(() =>
            {
                var releaseNotes = ReadAllLines(RootDirectory / "CHANGELOG.md")
                        .SkipWhile(x => !x.StartsWith("##"))
                        .Skip(count: 1)
                        .TakeWhile(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => $"\u2022{x.TrimStart('-')}")
                        .JoinNewLine();

                GlobFiles(SourceDirectory, "*.nuspec")
                        .ForEach(x => NuGetPack(s => DefaultNuGetPack
                                .SetTargetPath(x)
                                .SetBasePath(SourceDirectory)
                                .SetProperty("wave", GetWaveVersion(ProjectFile) + ".0")
                                .SetProperty("currentyear", DateTime.Now.Year.ToString())
                                .SetProperty("releasenotes", releaseNotes)
                                .EnableNoPackageAnalysis()));
            });

    Target Push => _ => _
            .DependsOn(Pack)
            .Requires(() => ApiKey)
            .Requires(() => Configuration.EqualsOrdinalIgnoreCase("Release"))
            .Executes(() =>
            {
                GlobFiles(OutputDirectory, "*.nupkg")
                        .ForEach(x => NuGetPush(s => s
                                .SetTargetPath(x)
                                .SetSource(Source)
                                .SetApiKey(ApiKey)));
            });

    static string GetWaveVersion (string projectFile)
    {
        var fullWaveVersion = GetLocalInstalledPackages(projectFile, includeDependencies: true)
                .SingleOrDefault(x => x.Id == "Wave").NotNull("fullWaveVersion != null").Version.ToString();
        return fullWaveVersion.Substring(startIndex: 0, length: fullWaveVersion.IndexOf(value: '.'));
    }
}