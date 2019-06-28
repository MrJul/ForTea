using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation
{
	public sealed class T4RoslynCompilationFailure : IT4RoslynCompilationResult
	{
		[NotNull, ItemNotNull]
		private IList<string> Errors { get; }

		public T4RoslynCompilationFailure([NotNull, ItemNotNull] IList<string> errors) => Errors = errors;

		public async Task SaveResultsAsync(IProjectFile destination)
		{
			using (var stream = destination.CreateWriteStream())
			using (var writer = new StreamWriter(stream))
			{
				foreach (string error in Errors)
				{
					await writer.WriteLineAsync(error);
				}
			}
		}
	}
}
