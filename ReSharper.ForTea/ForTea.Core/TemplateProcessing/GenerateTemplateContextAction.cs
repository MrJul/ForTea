using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
/*
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
*/
	public class GenerateTemplateContextAction : ContextActionBase
	{
		[NotNull] private const string Message = "Execute T4 design-time template";
		private IT4File File { get; }

		public GenerateTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) =>
			File = dataProvider.PsiFile as IT4File;

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl =>
		{
		};

		public override string Text => Message;
		public override bool IsAvailable(IUserDataHolder cache) => File != null;
	}
}
