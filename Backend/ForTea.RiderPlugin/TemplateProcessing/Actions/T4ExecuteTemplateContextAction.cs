using System;
using JetBrains.Annotations;
using JetBrains.Application.CommandProcessing;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;

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

		public override void Execute(ISolution solution, ITextControl textControl)
		{
			using (CompilationContextCookie.GetOrCreate(textControl.GetContext(solution)))
			{
				var psiServices = solution.GetPsiServices();
				psiServices.Files.AssertAllDocumentAreCommitted();

				var documentSettings = solution.GetComponent<DocumentSettings>();
				var commandProcessor = solution.GetComponent<ICommandProcessor>();
				var taskExecutor = solution.GetComponent<UITaskExecutor>();
				string actionText = Text.NotNull("Text != null");

				using (commandProcessor.UsingCommand(actionText))
				using (documentSettings.WithOpenDocumentAfterModification(true))
				{
					bool success = taskExecutor.SingleThreaded.ExecuteTask(actionText, TaskCancelable.Yes, progress =>
					{
						psiServices.Files.AssertAllDocumentAreCommitted();
						if (progress.IsCanceled) return;
						DoExecute(solution, progress);
					});

					if (!success) ActionCancelled(textControl);
				}
			}
		}

		private void DoExecute([NotNull] ISolution solution, IProgressIndicator progress) =>
			solution.GetComponent<IT4TemplateExecutionManager>().Execute(File.NotNull(), progress);

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => null;

		public override string Text => Message;
	}
}
