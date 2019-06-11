using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TextTemplating.VSHost;

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
			ProjectInfo info
		);

		void InvalidateAssemblies(
			[NotNull] T4FileDataDiff dataDiff,
			ref bool hasChanges,
			[NotNull] ProjectInfo info,
			[NotNull] T4AssemblyReferenceManager referenceManager
		);
	}
}
