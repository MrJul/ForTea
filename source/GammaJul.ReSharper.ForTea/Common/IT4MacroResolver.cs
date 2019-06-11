using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea
{
	public interface IT4MacroResolver
	{
		/// <summary>Resolves new VS macros, like $(SolutionDir), found in include or assembly directives.</summary>
		/// <param name="macros">The list of macro names (eg SolutionDir) to resolve.</param>
		/// <param name="info">Context in which to resolve macros</param>
		/// <returns>Resolved macros</returns>
		IReadOnlyDictionary<string, string> Resolve(
			[NotNull] [ItemNotNull] IEnumerable<string> macros,
			T4TemplateInfo info
		);

		void InvalidateAssemblies(
			[NotNull] T4FileDataDiff dataDiff,
			ref bool hasChanges,
			[NotNull] T4TemplateInfo info,
			[NotNull] T4AssemblyReferenceManager referenceManager
		);
	}
}
