using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
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

		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private IShellLocks Locks { get; }

		[NotNull]
		private string TargetExtension { get; }

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
			var solution = dataProvider.Solution;
			Manager = solution.GetComponent<T4DirectiveInfoManager>();
			Locks = solution.Locks;
			TargetExtension = File.GetTargetExtension(Manager);
			DestinationFileName = FileName.WithOtherExtension(TargetExtension);
		}

		protected override Action<ITextControl> ExecutePsiTransaction(
			ISolution solution,
			IProgressIndicator progress
		) => textControl => solution.InvokeUnderTransaction(cookie =>
		{
			Check();
			var destinationFile = GetOrCreateDestinationFile(cookie);
			var definition = solution.GetLifetime().CreateNested();
			LaunchProgress(definition, progress, solution);
			Locks.ExecuteOrQueueEx("T4 template execution", () => PerformBackgroundWork(definition, destinationFile, progress));
			cookie.Commit(progress);
		});

		private void LaunchProgress(
			LifetimeDefinition definition,
			[NotNull] IProgressIndicator progress,
			[NotNull] ISolution solution
		)
		{
			if (!(progress is IProgressIndicatorModel model)) return;
			// todo remove when protocol-independent IRiderBackgroundTaskHost is introduced
#pragma warning disable 618
			if (Shell.Instance.IsTestShell) return;
#pragma warning restore 618
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

		private void PerformBackgroundWork(
			[NotNull] LifetimeDefinition definition,
			[NotNull] IProjectFile destination,
			[NotNull] IProgressIndicator progress
		)
		{
			progress.CurrentItemText = "Generating code";
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
			var result = manager.Compile(metadataReferences);

			progress.CurrentItemText = "Executing code";
			result.SaveResults(definition.Lifetime, destination);
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
