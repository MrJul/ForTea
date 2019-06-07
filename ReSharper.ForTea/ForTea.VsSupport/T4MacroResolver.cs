using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.VsSupport
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
			T4TemplateInfo info
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
			T4TemplateInfo info,
			T4AssemblyReferenceManager referenceManager
		)
		{
			using (AssemblyResolver.Prepare(info))
			{
				// removes the assembly references from the old assembly directives
				foreach (string assembly in
					from it in dataDiff.RemovedAssemblies select AssemblyResolver.Resolve(info, it)
				)
				{
					if (!referenceManager.References.TryGetValue(assembly, out IAssemblyCookie cookie))
						continue;

					referenceManager.References.Remove(assembly);
					hasChanges = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives
				foreach (string addedAssembly in
					from it in dataDiff.AddedAssemblies select AssemblyResolver.Resolve(info, it)
				)
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
		private static IVsBuildMacroInfo TryGetVsBuildMacroInfo([NotNull] T4TemplateInfo info) =>
			Utils.TryGetVsHierarchy(info) as IVsBuildMacroInfo;
	}
}
