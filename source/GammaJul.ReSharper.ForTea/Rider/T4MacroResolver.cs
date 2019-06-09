using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Rider
{
	// TODO: resolve macros
	[ShellComponent]
	public sealed class T4MacroResolver : IT4MacroResolver
	{
		public IReadOnlyDictionary<string, string> Resolve(IEnumerable<string> macros) =>
			EmptyDictionary<string, string>.InstanceReadOnly;
	}
}
