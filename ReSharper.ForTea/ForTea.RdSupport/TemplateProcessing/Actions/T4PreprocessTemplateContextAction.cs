using System;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Extension;
using JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.TextControl;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "PreprocessTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4PreprocessTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] public const string DefaultDestinationFileName = "TextTransformation.cs";
		[NotNull] private const string Message = "Preprocess T4 template";

		protected override string DestinationFileName =>
			FileName?.WithOtherExtension(T4CSharpCodeGenerationUtils.PreprocessResultExtension)
			?? DefaultDestinationFileName;

		public T4PreprocessTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
		}

		[NotNull]
		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => _ => solution.InvokeUnderTransaction(cookie =>
		{
			if (File == null) return;
			var manager = solution.GetComponent<T4DirectiveInfoManager>();
			var generator = new T4CSharpCodeGenerator(File, manager);
			string message = generator.Generate().Builder.ToString();
			var destinationFile = GetOrCreateDestinationFile(cookie);
			if (destinationFile == null) return;
			// TODO: use better writing methods
			using (var writeStream = destinationFile.CreateWriteStream())
			{
				writeStream.WriteUtf8(message);
			}

			cookie.Commit(progress);
		});

		public override string Text => Message;
	}
}
