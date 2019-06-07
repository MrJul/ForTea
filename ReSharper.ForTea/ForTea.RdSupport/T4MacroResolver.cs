using System.Collections.Generic;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Application;
using JetBrains.Util;

namespace JetBrains.ForTea.RdSupport
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
