using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions.RoslynCompilation;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Generators;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Host.Features.BackgroundTasks;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
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

		private ILogger Logger { get; } = Util.Logging.Logger.GetLogger<T4ExecuteTemplateContextAction>();

		/// See
		/// <see cref="T4FileBasedContextActionBase">
		/// base class
		/// </see>
		/// constructor for details
		[SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized"),
		 SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse"),
		 SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
		public T4ExecuteTemplateContextAction([NotNull] LanguageIndependentContextActionDataProvider dataProvider) :
			base(dataProvider)
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
			var stopwatch = Stopwatch.StartNew();
			Check();
			var destinationFile = GetOrCreateDestinationFile(cookie);
			var definition = solution.GetLifetime().CreateNested();
			LaunchProgress(definition, progress, solution);
			PerformBackgroundWorkAsync(definition, destinationFile, progress);
			stopwatch.Stop();
			if (stopwatch.ElapsedMilliseconds >= 50)
				Logger.Warn($"Performance warning. Starting background task took too long: {stopwatch.Elapsed}");

			cookie.Commit(progress);
		});

		private void LaunchProgress(
			LifetimeDefinition definition,
			[NotNull] IProgressIndicator progress,
			[NotNull] ISolution solution
		)
		{
			if (!(progress is IProgressIndicatorModel model)) return;
			const string title = "Executing T4 Template";
			progress.Start(1);
			progress.Advance();
			progress.TaskName = title;
			progress.CurrentItemText = "Preparing";
			var task = RiderBackgroundTaskBuilder
				.FromProgressIndicator(model)
				.WithTitle(title)
				.AsCancelable(definition.Terminate)
				.Build();
			solution.GetComponent<RiderBackgroundTaskHost>().AddNewTask(definition.Lifetime, task);
		}

		private async void PerformBackgroundWorkAsync(
			[NotNull] LifetimeDefinition definition,
			[NotNull] IProjectFile destination,
			[NotNull] IProgressIndicator progress)
		{
			progress.CurrentItemText = "Generating code";
			var result = await Task.Run(() =>
			{
				string code;
				IEnumerable<MetadataReference> metadataReferences;
				using (ReadLockCookie.Create())
				{
					var generator = new T4CSharpExecutableCodeGenerator(File, Manager);
					code = generator.Generate().RawText;
					metadataReferences = ExtractReferences();
				}

				var manager = new T4RoslynCompilationManager(definition.Lifetime, code, Solution);
				progress.CurrentItemText = "Compiling code";
				return manager.Compile(metadataReferences);
			}, definition.Lifetime.ToCancellationToken());

			progress.CurrentItemText = "Executing code";
			await result.SaveResultsAsync(definition.Lifetime, destination);
			definition.Terminate();
		}

		private IEnumerable<MetadataReference> ExtractReferences()
		{
			var psiModule = PsiSourceFile.PsiModule;
			using (CompilationContextCookie.GetOrCreate(psiModule.GetResolveContextEx(ProjectFile)))
			{
				return Solution
					.GetComponent<IPsiModules>()
					.GetModuleReferences(psiModule)
					.Select(it => it.Module)
					.OfType<IAssemblyPsiModule>()
					.Select(it => it.Assembly)
					.SelectNotNull(it => it.Location)
					.Select(it => MetadataReference.CreateFromFile(it.FullPath));
			}
		}

		public override string Text => Message;
	}
}
