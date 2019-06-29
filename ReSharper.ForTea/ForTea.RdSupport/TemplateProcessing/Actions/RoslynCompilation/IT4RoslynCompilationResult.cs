using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation
{
	public interface IT4RoslynCompilationResult
	{
		Task SaveResultsAsync(Lifetime lifetime, [NotNull] IProjectFile destination);
	}
}
