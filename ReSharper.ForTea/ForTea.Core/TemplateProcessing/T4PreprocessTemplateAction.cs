using System;
using System.IO;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;
using static GammaJul.ForTea.Core.TemplateProcessing.T4CSharpCodeGenerationUtils;

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

		[NotNull]
		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => _ =>
		{
			var manager = solution.GetComponent<T4DirectiveInfoManager>();

			var generator = new T4CSharpCodeGenerator(File, manager);
			string newFilePath = GetDestinationFilePath();
			string message = generator.Generate().Builder.ToString();

			WriteData(newFilePath, message);
		};

		[NotNull]
		private string GetDestinationFilePath()
		{
			var initialPsiSourceFile = File.GetSourceFile();
			var initialProjectFile = initialPsiSourceFile.ToProjectFile();
			string newFileName = initialPsiSourceFile?.Name.WithExtension(DefaultTargetExtension);
			return initialProjectFile?.Location.Parent.Combine(newFileName).FullPath
			       ?? throw new NullReferenceException();
		}

		private static void WriteData([NotNull] string newFilePath, [NotNull] string message)
		{
			using (var stream = System.IO.File.Create(newFilePath))
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(message);
			}
		}

		public override string Text => Message;
		public override bool IsAvailable(IUserDataHolder cache) => File != null;
	}
}
