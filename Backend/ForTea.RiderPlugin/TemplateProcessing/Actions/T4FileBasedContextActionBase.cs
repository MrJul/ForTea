using System;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.CommandProcessing;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.TextControl;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
{
	public abstract class T4FileBasedContextActionBase : ContextActionBase
	{
		[NotNull]
		private LanguageIndependentContextActionDataProvider Provider { get; }

		[CanBeNull]
		protected IT4File File => FindT4File(Provider);

		protected T4FileBasedContextActionBase([NotNull] LanguageIndependentContextActionDataProvider provider) =>
			Provider = provider;

		[CanBeNull]
		private static IT4File FindT4File([NotNull] LanguageIndependentContextActionDataProvider provider) =>
			provider.SourceFile.GetPsiFile<T4Language>(provider.DocumentCaret) as IT4File;

		public sealed override bool IsAvailable(IUserDataHolder cache) => File != null;

		public sealed override void Execute(ISolution solution, ITextControl textControl)
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

		protected sealed override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => null;

		protected abstract void DoExecute([NotNull] ISolution solution, [NotNull] IProgressIndicator progress);
	}
}
