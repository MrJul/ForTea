using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Lifetimes;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TemplateExecutionManager
	{
		string Execute(
			[NotNull] IT4File file,
			[CanBeNull] IProgressIndicator progress = null,
			Lifetime? outerLifetime = null
		);
	}
}
