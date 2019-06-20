using System.Collections.Generic;
using System.Collections.ObjectModel;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace JetBrains.ForTea.RdSupport
{
	// TODO: resolve more macros
	[SolutionComponent]
	public sealed class T4MacroResolver : IT4MacroResolver
	{
		[NotNull]
		private ISolution Solution { get; }

		private IReadOnlyDictionary<string, string> KnownMacros { get; }

		public IReadOnlyDictionary<string, string> Resolve(
			IEnumerable<string> macros,
			T4TemplateInfo info
		) => KnownMacros;

		public void InvalidateAssemblies(
			T4FileDataDiff dataDiff,
			ref bool hasChanges,
			T4TemplateInfo info,
			T4AssemblyReferenceManager referenceManager
		)
		{
		}

		public T4MacroResolver([NotNull] ISolution solution)
		{
			Solution = solution;
			KnownMacros = new Dictionary<string, string>
			{
				{"SolutionDir", Solution.SolutionDirectory.FullPath}
			};
		}
	}
}
