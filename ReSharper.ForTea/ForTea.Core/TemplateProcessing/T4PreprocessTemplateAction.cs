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
	[ContextAction(
		Name = "PreprocessTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public class T4PreprocessTemplateAction : ContextActionBase
	{
		[NotNull] private const string Message = "Preprocess T4 template";
		private IT4File File { get; }

		public T4PreprocessTemplateAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) =>
			File = dataProvider.PsiFile as IT4File;

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => _ => throw new NotImplementedException();

		public override string Text => Message;
		public override bool IsAvailable(IUserDataHolder cache) => File != null;
	}
}
