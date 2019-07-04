using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Processes;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions.RoslynCompilation
{
	public sealed class T4RoslynCompilationSuccess : IT4RoslynCompilationResult
	{
		[NotNull]
		private FileSystemPath ExecutablePath { get; }

		[NotNull]
		private ISolution Solution { get; }

		public T4RoslynCompilationSuccess(
			[NotNull] FileSystemPath executablePath,
			[NotNull] ISolution solution
		)
		{
			ExecutablePath = executablePath;
			Solution = solution;
		}

		public async Task SaveResultsAsync(Lifetime lifetime, IProjectFile destination)
		{
			var process = LaunchProcess(lifetime);

			using (var stream = destination.CreateWriteStream())
			{
				lifetime.ThrowIfNotAlive();
				await process.StandardOutput.BaseStream.CopyToAsync(stream);
				lifetime.ThrowIfNotAlive();
				await process.StandardError.BaseStream.CopyToAsync(stream);
				lifetime.ThrowIfNotAlive();
				await process.WaitForExitAsync();
			}
		}

		private Process LaunchProcess(Lifetime lifetime)
		{
			var patcher = Solution.GetComponent<RiderProcessStartInfoPatcher>();
			var startInfo = new JetProcessStartInfo(new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = ExecutablePath.FullPath,
				CreateNoWindow = true
			});

			var request = JetProcessRuntimeRequest.CreateFramework();
			var patchResult = patcher.Patch(startInfo, request);
			var process = new Process
			{
				StartInfo = patchResult.GetPatchedInfoOrThrow().ToProcessStartInfo()
			};
			lifetime.OnTermination(process);
			lifetime.Bracket(
				() => process.Start(),
				() => Logger.CatchSilent(() => { if (!process.HasExited) process.KillTree(); })
			);
			return process;
		}
	}
}
