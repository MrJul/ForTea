using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation
{
	public interface IT4RoslynCompilationResult
	{
		Task SaveResultsAsync([NotNull] IProjectFile destination);
	}
}
