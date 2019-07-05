using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions.RoslynCompilation
{
	public interface IT4RoslynCompilationResult
	{
		void SaveResults(Lifetime lifetime, [NotNull] IProjectFile destination);
	}
}
