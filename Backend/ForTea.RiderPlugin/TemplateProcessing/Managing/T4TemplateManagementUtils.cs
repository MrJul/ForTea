using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util.Logging;
using JetBrains.Util.Threading;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public static class T4TemplateManagementUtils
	{
		[Obsolete]
		public static void RunWithProgress([NotNull] Action<IProgressIndicator> action,
			[NotNull] string taskName,
			[NotNull] ISolution solution)
		{
			using (ReadLockCookie.Create())
			{
				var psiManager = solution.GetPsiServices();
				psiManager.Files.AssertAllDocumentAreCommitted();
				psiManager.Caches.Update();
			}

			try
			{
				Shell.Instance
					.GetComponent<UITaskExecutor>()
					.FreeThreaded
					.ExecuteTask(taskName, TaskCancelable.Yes, action);
			}
			catch (Exception e) when (e.IsOperationCanceled())
			{
				throw;
			}
			catch (Exception e)
			{
				Logger.LogException(e);
			}
		}
	}
}
