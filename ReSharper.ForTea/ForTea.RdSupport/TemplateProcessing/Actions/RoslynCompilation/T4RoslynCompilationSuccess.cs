using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application.Processes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.Processes;
using JetBrains.Util;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation
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

		// TODO: redirect output instead of storing all the data in a single string. It might be huge.
		public async Task SaveResultsAsync(IProjectFile destination)
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

			// TODO: should this one be used?
			var request = JetProcessRuntimeRequest.CreateFramework();
			var patchResult = patcher.Patch(startInfo, request);
			var process = new Process
			{
				StartInfo = patchResult.GetPatchedInfoOrThrow().ToProcessStartInfo()
			};
			process.Start();

			string output = await process.StandardOutput.ReadToEndAsync();
			if (output.IsNullOrEmpty())
			{
				output = await process.StandardError.ReadToEndAsync();
			}

			process.WaitForExit();
			using (var stream = destination.CreateWriteStream())
			using (var writer = new StreamWriter(stream))
			{
				await writer.WriteAsync(output);
			}
		}
	}
}
