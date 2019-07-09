using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions
{
	public abstract class T4FileBasedContextActionBase : ContextActionBase
	{
		[NotNull]
		private LanguageIndependentContextActionDataProvider Provider { get; }

		[CanBeNull]
		protected IT4File File => FindT4File(Provider);

		protected T4FileBasedContextActionBase([NotNull] LanguageIndependentContextActionDataProvider provider) =>
			Provider = provider;

		[CanBeNull]
		private static IT4File FindT4File([NotNull] LanguageIndependentContextActionDataProvider provider) =>
			provider.SourceFile.GetPsiFile<T4Language>(provider.DocumentCaret) as IT4File;

		public sealed override bool IsAvailable(IUserDataHolder cache) => File != null;
	}
}
