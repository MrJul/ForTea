using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4ExecuteTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string Message = "Execute T4 design-time template";

		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider)
		{
		}

		protected override void DoExecute(ISolution solution, IProgressIndicator progress)
		{
			string result = solution.GetComponent<IT4TemplateExecutionManager>().Execute(File.NotNull(), progress);
			var fileManager = solution.GetComponent<IT4TargetFileManager>();
			fileManager.SaveResults(result, File);
		}

		public override string Text => Message;
	}
}
