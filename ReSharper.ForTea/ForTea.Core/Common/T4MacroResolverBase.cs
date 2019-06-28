using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Common
{
	public abstract class T4MacroResolverBase : IT4MacroResolver
	{
		[NotNull]
		private IT4AssemblyNamePreprocessor AssemblyNamePreprocessor { get; }

		protected T4MacroResolverBase([NotNull] IT4AssemblyNamePreprocessor preprocessor) =>
			AssemblyNamePreprocessor = preprocessor;

		public abstract IReadOnlyDictionary<string, string> Resolve(IEnumerable<string> macros, T4TemplateInfo info);

		public void InvalidateAssemblies(
			T4FileDataDiff dataDiff,
			ref bool hasChanges,
			T4TemplateInfo info,
			T4AssemblyReferenceManager referenceManager
		)
		{
			using (AssemblyNamePreprocessor.Prepare(info))
			{
				// removes the assembly references from the old assembly directives
				foreach (string assembly in dataDiff
					.RemovedAssemblies
					.Select(it => AssemblyNamePreprocessor.Preprocess(info, it)))
				{
					bool assemblyExisted = referenceManager.References.TryGetValue(assembly, out var cookie);
					if (!assemblyExisted) continue;
					referenceManager.References.Remove(assembly);
					hasChanges = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives
				foreach (var _ in dataDiff
					.AddedAssemblies
					.Select(it => AssemblyNamePreprocessor.Preprocess(info, it))
					.Where(addedAssembly => !referenceManager.References.ContainsKey(addedAssembly))
					.Select(referenceManager.TryAddReference)
					.Where(cookie => cookie != null))
				{
					hasChanges = true;
				}
			}
		}
	}
}
