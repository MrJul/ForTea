using System;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	public interface IT4AssemblyNamePreprocessor
	{
		[NotNull]
		string Preprocess([NotNull] IProjectFile file, [NotNull] string assemblyName);

		[NotNull]
		IDisposable Prepare([NotNull] IProjectFile file);
	}
}
