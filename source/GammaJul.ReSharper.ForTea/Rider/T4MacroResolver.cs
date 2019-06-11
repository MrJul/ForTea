using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Common;
using JetBrains.Application;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Rider
{
	// TODO: resolve macros
	[ShellComponent]
	public sealed class T4MacroResolver : IT4MacroResolver
	{
		public IReadOnlyDictionary<string, string> Resolve(
			IEnumerable<string> macros,
			T4TemplateInfo info
		) => EmptyDictionary<string, string>.InstanceReadOnly;

		public void InvalidateAssemblies(
			T4FileDataDiff dataDiff,
			ref bool hasChanges,
			T4TemplateInfo info,
			T4AssemblyReferenceManager referenceManager
		)
		{
		}
	}
}
