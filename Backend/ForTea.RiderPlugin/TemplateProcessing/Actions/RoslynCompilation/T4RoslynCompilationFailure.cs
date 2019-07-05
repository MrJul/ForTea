using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Update;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions.RoslynCompilation
{
	public sealed class T4RoslynCompilationFailure : IT4RoslynCompilationResult
	{
		[NotNull, ItemNotNull]
		private IList<string> Errors { get; }

		public T4RoslynCompilationFailure([NotNull, ItemNotNull] IList<string> errors) => Errors = errors;

		public void SaveResults(Lifetime lifetime, IProjectFile destination)
		{
			using (destination.UsingWriteLockForUpdate())
			{
				using (var stream = destination.CreateWriteStream())
				using (var writer = new StreamWriter(stream))
				{
					writer.WriteLine("ErrorGeneratingOutput");
				}
				// TODO: use more elegant way to show error messages
				MessageBox.ShowError(Errors.Join("\n"), "Could not compile template");
			}
		}
	}
}
