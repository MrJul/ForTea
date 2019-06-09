using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace GammaJul.ReSharper.ForTea.VisualStudio
{
	[ShellComponent]
	public sealed class T4MacroResolver : IT4MacroResolver
	{
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
					value = MSBuildExtensions.GetStringValue(TryGetVsHierarchy(info), macro, null);
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
			ITextTemplatingComponents components,
			ref bool hasChanges,
			ProjectInfo info,
			T4AssemblyReferenceManager referenceManager
		)
		{
			using (components.With(TryGetVsHierarchy(info), info.ProjectFile.Location))
			{
				// removes the assembly references from the old assembly directives
				foreach (string removedAssembly in dataDiff.RemovedAssemblies)
				{
					string assembly = removedAssembly;
					if (components != null)
						assembly = components.Host.ResolveAssemblyReference(assembly);

					if (!referenceManager.References.TryGetValue(assembly, out IAssemblyCookie cookie))
						continue;

					referenceManager.References.Remove(assembly);
					hasChanges = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives
				foreach (string addedAssembly in dataDiff.AddedAssemblies)
				{
					string assembly = addedAssembly;
					if (components != null)
						assembly = components.Host.ResolveAssemblyReference(assembly);

					if (assembly == null)
						continue;

					if (referenceManager.References.ContainsKey(assembly))
						continue;

					IAssemblyCookie cookie = referenceManager.TryAddReference(assembly);
					if (cookie != null)
						hasChanges = true;
				}
			}
		}

		/// <summary>The <see cref="IVsHierarchy"/> representing the project file normally implements <see cref="IVsBuildMacroInfo"/>.</summary>
		/// <returns>An instance of <see cref="IVsBuildMacroInfo"/> if found.</returns>
		[CanBeNull]
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private IVsBuildMacroInfo TryGetVsBuildMacroInfo([NotNull] ProjectInfo info) => TryGetVsHierarchy(info) as IVsBuildMacroInfo;

		[CanBeNull]
		private IVsHierarchy TryGetVsHierarchy([NotNull] ProjectInfo info) => info.Solution
			.TryGetComponent<ProjectModelSynchronizer>()
			?.TryGetHierarchyItemByProjectItem(info.Project, false)
			?.Hierarchy;
	}
}
