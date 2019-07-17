using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
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

		public void CreateDestinationFileIfNeeded(IT4File file, string targetExtension = null)
		{
			Solution.Locks.AssertReadAccessAllowed();
			string targetFileName = GetTargetFileName(file, targetExtension);
			var projectFile = file.GetSourceFile().ToProjectFile().NotNull();
			var folder = projectFile.ParentFolder.NotNull();
			if (folder.GetSubItems(targetFileName).SingleItem() is IProjectFile) return;

			Solution.InvokeUnderTransaction(cookie =>
			{
				var targetLocation = folder.Location.Combine(targetFileName);
				if (!cookie.CanAddFile(folder, targetLocation, out string reason))
					throw new InvalidOperationException(reason);
				cookie.AddFile(folder, targetLocation);
				cookie.Commit(NullProgressIndicator.Create());
			});
		}

		[NotNull]
		private FileSystemPath GetDestinationPath([NotNull] IT4File file, [CanBeNull] string targetExtension = null)
		{
			if (targetExtension == null) targetExtension = file.GetTargetExtension(Manager);
			var sourceFile = file.GetSourceFile().NotNull();
			string targetFileName = sourceFile.Name.WithOtherExtension(targetExtension);
			var targetLocation = sourceFile.GetLocation().Parent.Combine(targetFileName);
			return targetLocation;
		}

		public FileSystemPath SaveResults(string result, IT4File file, string targetExtension = null)
		{
			var destination = GetDestinationPath(file, targetExtension);
			// We are so unsafe
			// TODO: fix endings!
			destination.WriteAllText(result.Replace("\r\n", "\n"));
			return destination;
		}
	}
}
