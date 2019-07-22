using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Resources.Shell;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "PreprocessTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4PreprocessTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string PreprocessResultExtension = "cs";
		[NotNull] private const string Message = "Preprocess T4 template";

		public T4PreprocessTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider)
		{
		}

		protected override void DoExecute(ISolution solution, IProgressIndicator progress)
		{
			var directiveInfoManager = solution.GetComponent<T4DirectiveInfoManager>();
			string message = new T4CSharpCodeGenerator(File, directiveInfoManager).Generate().RawText;
			var fileManager = solution.GetComponent<IT4TargetFileManager>();
			using (WriteLockCookie.Create())
			{
				fileManager.SaveResults(message, File, PreprocessResultExtension);
			}
		}

		public override string Text => Message;
	}
}
