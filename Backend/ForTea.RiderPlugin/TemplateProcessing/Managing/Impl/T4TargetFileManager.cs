using System;
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

		[NotNull]
		private IShellLocks Locks => Solution.Locks;

		public T4TargetFileManager([NotNull] T4DirectiveInfoManager manager, [NotNull] ISolution solution)
		{
			Manager = manager;
			Solution = solution;
		}

		private string GetTargetFileName([NotNull] IT4File file, [CanBeNull] string targetExtension)
		{
			Locks.AssertReadAccessAllowed();
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
			Locks.AssertWriteAccessAllowed();
			var existingFile = FindExistingFile(file, targetExtension);
			if (existingFile != null) return existingFile;
			return CreateDestinationFile(cookie, file, GetTargetFileName(file, targetExtension));
		}

		private IProjectFile FindExistingFile(IT4File file, string targetExtension) => file
			.GetSourceFile()
			.ToProjectFile()
			?.ParentFolder
			?.GetSubItems(GetTargetFileName(file, targetExtension))
			.SingleOrDefault() as IProjectFile;

		private IProjectFile CreateDestinationFile(
			[NotNull] IProjectModelTransactionCookie cookie,
			[NotNull] IT4File file,
			[NotNull] string targetFileName
		)
		{
			Locks.AssertWriteAccessAllowed();
			var projectFile = file.GetSourceFile().ToProjectFile().NotNull();
			var folder = projectFile.ParentFolder.NotNull();
			var targetLocation = folder.Location.Combine(targetFileName);
			var parameters = T4MSBuildProjectUtil.CreateTemplateMetadata(projectFile);
			return cookie.AddFile(folder, targetLocation, parameters);
		}

		[NotNull]
		private FileSystemPath SelectDestination([NotNull] IT4File file, [CanBeNull] string targetExtension)
		{
			string targetFileName = GetTargetFileName(file, targetExtension);
			return file.GetSourceFile().ToProjectFile()?.ParentFolder?.Location.Combine(targetFileName)
			       ?? throw new InvalidOperationException();
		}

		public FileSystemPath SaveResults(string result, IT4File file, string targetExtension = null)
		{
			Locks.AssertReadAccessAllowed();
			if (Locks.IsWriteAccessAllowed()) return OperateUnderWriteLock(result, file, targetExtension);
			return OperateWithoutWriteLock(result, file, targetExtension);
		}

		[NotNull]
		private FileSystemPath OperateWithoutWriteLock(string result, IT4File file, string targetExtension)
		{
			Locks.AssertReadAccessAllowed();
			Assertion.Assert(!Locks.IsWriteAccessAllowed(), "!Locks.IsWriteAccessAllowed()");
			// We are being called from SingleFileCustomToolManager, that will take care of caches
			var destination = SelectDestination(file, targetExtension);
			destination.WriteAllText(result);
			if (FindExistingFile(file, targetExtension) != null) return destination;
			// Transaction cannot be performed right away, so add file to project model later
			Locks.ExecuteOrQueueEx(Solution.GetLifetime(), "Creating file for T4 execution results", () =>
				Solution.InvokeUnderTransaction(cookie => CreateDestinationFile(cookie, file, targetExtension))
			);
			return destination;
		}

		private FileSystemPath OperateUnderWriteLock(
			[NotNull] string result,
			[NotNull] IT4File file,
			[CanBeNull] string targetExtension
		)
		{
			Locks.AssertWriteAccessAllowed();
			// We are being invoked from context action
			// and are responsible for performing transaction and invalidating caches
			FileSystemPath destinationLocation = null;
			IProjectFile destination = null;
			Solution.InvokeUnderTransaction(cookie =>
			{
				destination = CreateDestinationFileIfNeeded(cookie, file, targetExtension);
				destinationLocation = destination.Location;
				destinationLocation.WriteAllText(result);
			});
			Solution.GetComponent<DocumentHost>().SyncDocumentsWithFiles(destinationLocation);
			var sourceFile = destination.ToSourceFile();
			if (sourceFile != null) SyncSymbolCaches(sourceFile);
			RefreshFiles(Solution, destinationLocation);
			return destinationLocation;
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
