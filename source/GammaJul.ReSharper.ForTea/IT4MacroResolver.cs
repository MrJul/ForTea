using System.Collections.Generic;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea
{
	public interface IT4MacroResolver
	{
		/// <summary>Resolves new VS macros, like $(SolutionDir), found in include or assembly directives.</summary>
		/// <param name="macros">The list of macro names (eg SolutionDir) to resolve.</param>
		/// <returns>Resolved macros</returns>
		IReadOnlyDictionary<string, string> Resolve([NotNull] [ItemNotNull] IEnumerable<string> macros);
	}
}
