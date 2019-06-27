using System.Collections.Generic;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RdSupport
{
	// TODO: resolve more macros
	[SolutionComponent]
	public sealed class T4MacroResolver : T4MacroResolverBase
	{
		[NotNull]
		private ISolution Solution { get; }

		private IReadOnlyDictionary<string, string> KnownMacros { get; }

		public override IReadOnlyDictionary<string, string> Resolve(
			IEnumerable<string> macros,
			T4TemplateInfo info
		) => KnownMacros;

		public T4MacroResolver([NotNull] ISolution solution, [NotNull] IT4AssemblyResolver resolver) : base(resolver)
		{
			Solution = solution;
			KnownMacros = new Dictionary<string, string>
			{
				{"SolutionDir", Solution.SolutionDirectory.FullPath}
			};
		}
	}
}
