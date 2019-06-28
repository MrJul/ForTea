using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation
{
	public interface IT4RoslynCompilationResult
	{
		void SaveResults([NotNull] IProjectFile destination);
	}
}
