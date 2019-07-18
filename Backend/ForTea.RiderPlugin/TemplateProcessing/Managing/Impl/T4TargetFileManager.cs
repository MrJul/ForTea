using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
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

			var targetLocation = folder.Location.Combine(targetFileName);
			Assertion.Assert(
				cookie.CanAddFile(folder, targetLocation, out string reason),
				$"Could not add file to project model: {reason}");
			return cookie.AddFile(folder, targetLocation);
		}

		public FileSystemPath SaveResults(string result, IT4File file, string targetExtension = null)
		{
			FileSystemPath destinationLocation = null;
			file.GetSourceFile()?.GetSolution().InvokeUnderTransaction(cookie =>
			{
				var destination = CreateDestinationFileIfNeeded(cookie, file, targetExtension);
				destinationLocation = destination.Location;
				// TODO: fix endings!
				destinationLocation.WriteAllText(result.Replace("\r\n", "\n"));
			});
			return destinationLocation.NotNull("Could not ");
		}
	}
}
