using System;
using System.Diagnostics.CodeAnalysis;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation;
using JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.BackgroundTasks;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.TextControl;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions
{
	[ContextAction(
		Name = "ExecuteTemplate",
		Description = Message,
		Group = "T4",
		Disabled = false,
		Priority = 1)]
	public sealed class T4ExecuteTemplateContextAction : T4FileBasedContextActionBase
	{
		[NotNull] private const string Message = "Execute T4 design-time template";
		protected override string DestinationFileName { get; }

		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private string TargetExtension { get; }

		/// See
		/// <see cref="JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.T4FileBasedContextActionBase">
		/// base class
		/// </see>
		/// constructor for details
		[SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized"),
		 SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse"),
		 SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider.PsiFile as IT4File)
		{
			if (File == null) return;
			Manager = dataProvider.Solution.GetComponent<T4DirectiveInfoManager>();
			TargetExtension = File.GetTargetExtension(Manager);
			DestinationFileName = FileName.WithOtherExtension(TargetExtension);
		}

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			Check();
			var lifetimeDefinition = LaunchProgress(progress, solution);
			var destinationFile = GetOrCreateDestinationFile(cookie);
			PerformBackgroundWork(solution, destinationFile, lifetimeDefinition);
		});

		// TODO: add more informative messages to progress indicator
		[NotNull]
		private LifetimeDefinition LaunchProgress(
			[NotNull] IProgressIndicator progress,
			[NotNull] ISolution solution
		)
		{
			var solutionLifetime = solution.GetLifetime();
			var definition = solutionLifetime.CreateNested();
			if (!(progress is IProgressIndicatorModel model)) return definition;
			progress.Start(1);
			progress.Advance();
			var task = RiderBackgroundTaskBuilder
				.FromProgressIndicator(model)
				.AsIndeterminate()
				.WithHeader("Executing template")
				.Build();
			solution.GetComponent<RiderBackgroundTaskHost>().AddNewTask(definition.Lifetime, task);
			return definition;
		}

		private void PerformBackgroundWork(
			[NotNull] ISolution solution,
			[NotNull] IProjectFile destination,
			[NotNull] LifetimeDefinition definition
		)
		{
			var generator = new T4CSharpExecutableCodeGenerator(File, Manager);
			string code = generator.Generate().RawText;
			var manager = new T4RoslynCompilationManager(definition.Lifetime, solution, code, PsiSourceFile);
			var compilationResult = manager.Compile();
			compilationResult.SaveResults(destination);
			definition.Terminate();
		}

		public override string Text => Message;
	}
}
