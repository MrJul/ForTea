using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4MacroResolver
	{
		/// <summary>Resolves new VS macros, like $(SolutionDir), found in include or assembly directives.</summary>
		/// <param name="macros">The list of macro names (eg SolutionDir) to resolve.</param>
		/// <param name="file">Context in which to resolve macros</param>
		/// <returns>Resolved macros</returns>
		IReadOnlyDictionary<string, string> Resolve(
			[NotNull] [ItemNotNull] IEnumerable<string> macros,
			[NotNull] IProjectFile file
		);

		// TODO: move somewhere else. Wtf Macro resolver handles assemblies?
		void InvalidateAssemblies(
			[NotNull] T4FileDataDiff dataDiff,
			ref bool hasChanges,
			[NotNull] IProjectFile file,
			[NotNull] T4AssemblyReferenceManager referenceManager
		);
	}
}
