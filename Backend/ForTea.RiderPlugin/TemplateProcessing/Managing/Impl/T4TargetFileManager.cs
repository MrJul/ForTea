using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.Documents;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Caches.SymbolCache;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public class T4TargetFileManager : IT4TargetFileManager
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private ISolution Solution { get; }

		public T4TargetFileManager([NotNull] T4DirectiveInfoManager manager, [NotNull] ISolution solution)
		{
			Manager = manager;
			Solution = solution;
		}

		private string GetTargetFileName(IT4File file, string targetExtension)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			string name = sourceFile.Name;
			if (targetExtension == null) targetExtension = file.GetTargetExtension(Manager);
			return name.WithOtherExtension(targetExtension);
		}

		private IProjectFile CreateDestinationFileIfNeeded(
			IProjectModelTransactionCookie cookie,
			IT4File file,
			string targetExtension
		)
		{
			Solution.Locks.AssertReadAccessAllowed();
			string targetFileName = GetTargetFileName(file, targetExtension);
			var projectFile = file.GetSourceFile().ToProjectFile().NotNull();
			var folder = projectFile.ParentFolder.NotNull();
			if (folder.GetSubItems(targetFileName).SingleItem() is IProjectFile result) return result;
			return CreateDestinationFile(cookie, projectFile, targetFileName);
		}

		private static IProjectFile CreateDestinationFile(
			IProjectModelTransactionCookie cookie,
			IProjectFile projectFile,
			string targetFileName
		)
		{
			var folder = projectFile.ParentFolder.NotNull();
			var targetLocation = folder.Location.Combine(targetFileName);
			var parameters = T4MSBuildProjectUtil.CreateTemplateMetadata(projectFile);
			return cookie.AddFile(folder, targetLocation, parameters);
		}

		private FileSystemPath SelectDestination([NotNull] IT4File file, [CanBeNull] string targetExtension)
		{
			string targetFileName = GetTargetFileName(file, targetExtension);
			return file.GetSourceFile().ToProjectFile()?.ParentFolder?.Location.Combine(targetFileName);
		}

		public FileSystemPath SaveResults(string result, IT4File file, string targetExtension = null)
		{
			var solution = file.GetSourceFile()?.GetSolution();
			if (solution?.Locks.IsWriteAccessAllowed() == true)
			{
				// We are being invoked from context action
				// and are responsible for performing transaction and invalidating caches
				FileSystemPath destinationLocation = null;
				IProjectFile destination = null;
				solution.InvokeUnderTransaction(cookie =>
				{
					destination = CreateDestinationFileIfNeeded(cookie, file, targetExtension);
					destinationLocation = destination.Location;
					destinationLocation.WriteAllText(result);
				});
				solution.GetComponent<DocumentHost>().SyncDocumentsWithFiles(destinationLocation);
				var sourceFile = destination.ToSourceFile();
				if (sourceFile != null) SyncSymbolCaches(sourceFile);
				RefreshFiles(solution, destinationLocation);
				return destinationLocation;
			}

			// We are being called from
			// JetBrains.ReSharper.Host.Features.ProjectModel.CustomTools.SingleFileCustomToolManager,
			// that will take care of caches
			var destination1 = SelectDestination(file, targetExtension);
			destination1.WriteAllText(result);
			return destination1;
		}

		private static void RefreshFiles(ISolution solution, FileSystemPath destinationLocation)
		{
			var fileSystemModel = solution.GetProtocolSolution().GetFileSystemModel();
			solution.GetProtocolSolution()
				.Editors
				.SaveFiles
				.Start(new List<string> {destinationLocation.FullPath});
			fileSystemModel
				.RefreshPaths
				.Start(new RdRefreshRequest(new List<string> {destinationLocation.FullPath}, true));
		}

		private void SyncSymbolCaches([NotNull] IPsiSourceFile changedFile)
		{
			var changeManager = Solution.GetPsiServices().GetComponent<ChangeManager>();
			var invalidateCacheChange = new InvalidateCacheChange(
				Solution.GetComponent<SymbolCache>(),
				new[] {changedFile},
				true);

			using (WriteLockCookie.Create())
			{
				changeManager.OnProviderChanged(Solution, invalidateCacheChange, SimpleTaskExecutor.Instance);
			}
		}
	}
}
