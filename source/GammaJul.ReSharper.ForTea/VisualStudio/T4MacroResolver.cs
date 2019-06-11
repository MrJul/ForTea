using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GammaJul.ReSharper.ForTea.Common;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace GammaJul.ReSharper.ForTea.VisualStudio
{
	[ShellComponent]
	public sealed class T4MacroResolver : IT4MacroResolver
	{
		[NotNull]
		private IT4AssemblyResolver AssemblyResolver { get; }

		public T4MacroResolver(IT4AssemblyResolver assemblyResolver) =>
			AssemblyResolver = assemblyResolver;

		public IReadOnlyDictionary<string, string> Resolve(
			IEnumerable<string> macros,
			ProjectInfo info
		)
		{
			var result = new Dictionary<string, string>();
			IVsBuildMacroInfo vsBuildMacroInfo = null;
			foreach (string macro in macros)
			{
				if (vsBuildMacroInfo == null)
				{
					vsBuildMacroInfo = TryGetVsBuildMacroInfo(info);
					if (vsBuildMacroInfo == null)
					{
						Logger.LogError("Couldn't get IVsBuildMacroInfo");

						break;
					}
				}

				bool succeeded =
					HResultHelpers.SUCCEEDED(vsBuildMacroInfo.GetBuildMacroValue(macro, out string value))
				&& !string.IsNullOrEmpty(value);
				if (!succeeded)
				{
					value = MSBuildExtensions.GetStringValue(Utils.TryGetVsHierarchy(info), macro, null);
					succeeded = !string.IsNullOrEmpty(value);
				}

				if (succeeded)
				{
					result[macro] = value;
				}
			}

			return result;
		}

		public void InvalidateAssemblies(
			T4FileDataDiff dataDiff,
			ref bool hasChanges,
			ProjectInfo info,
			T4AssemblyReferenceManager referenceManager
		)
		{
			using (AssemblyResolver.Prepare(info))
			{
				// removes the assembly references from the old assembly directives
				foreach (string removedAssembly in dataDiff.RemovedAssemblies.Select(AssemblyResolver.Resolve))
				{
					if (!referenceManager.References.TryGetValue(removedAssembly, out IAssemblyCookie cookie))
						continue;

					referenceManager.References.Remove(removedAssembly);
					hasChanges = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives

				foreach (string addedAssembly in dataDiff.AddedAssemblies.Select(AssemblyResolver.Resolve))
				{
					if (referenceManager.References.ContainsKey(addedAssembly))
						continue;

					IAssemblyCookie cookie = referenceManager.TryAddReference(addedAssembly);
					if (cookie != null)
						hasChanges = true;
				}
			}
		}

		/// <summary>The <see cref="IVsHierarchy"/> representing the project file normally implements <see cref="IVsBuildMacroInfo"/>.</summary>
		/// <returns>An instance of <see cref="IVsBuildMacroInfo"/> if found.</returns>
		[CanBeNull]
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private static IVsBuildMacroInfo TryGetVsBuildMacroInfo([NotNull] ProjectInfo info) =>
			Utils.TryGetVsHierarchy(info) as IVsBuildMacroInfo;
	}
}
