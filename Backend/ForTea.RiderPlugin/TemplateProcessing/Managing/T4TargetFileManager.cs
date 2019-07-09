using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	[SolutionComponent]
	public class T4TargetFileManager : IT4TargetFileManager
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4TargetFileManager([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		public IProjectFile GetOrCreateDestinationFile(
			IT4File file,
			IProjectModelTransactionCookie cookie,
			string targetExtension = null
		)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			string name = sourceFile.Name;
			if (targetExtension == null) targetExtension = file.GetTargetExtension(Manager);
			string targetFileName = name.WithOtherExtension(targetExtension);
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var folder = projectFile.ParentFolder.NotNull();
			if (folder.GetSubItems(targetFileName).SingleItem() is IProjectFile existingResult) return existingResult;

			var targetLocation = folder.Location.Combine(targetFileName);
			if (!cookie.CanAddFile(folder, targetLocation, out string reason))
				throw new InvalidOperationException(reason);
			return cookie.AddFile(folder, targetLocation);
		}
	}
}
