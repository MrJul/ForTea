using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.ReSharperPlugin
{
	[ShellComponent]
	public sealed class T4MacroResolver : T4MacroResolverBase
	{
		public T4MacroResolver(IT4AssemblyNamePreprocessor preprocessor) : base(preprocessor)
		{
		}

		public override IReadOnlyDictionary<string, string> Resolve(
			IEnumerable<string> macros,
			IProjectFile file
		)
		{
			var result = new Dictionary<string, string>();
			IVsBuildMacroInfo vsBuildMacroInfo = null;
			foreach (string macro in macros)
			{
				if (vsBuildMacroInfo == null)
				{
					vsBuildMacroInfo = TryGetVsBuildMacroInfo(file);
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
					value = MSBuildExtensions.GetStringValue(Utils.TryGetVsHierarchy(file), macro, null);
					succeeded = !string.IsNullOrEmpty(value);
				}

				if (succeeded)
				{
					result[macro] = value;
				}
			}

			return result;
		}

		/// <summary>
		/// The <see cref="IVsHierarchy"/> representing the project file
		/// normally implements <see cref="IVsBuildMacroInfo"/>.
		/// </summary>
		/// <returns>An instance of <see cref="IVsBuildMacroInfo"/> if found.</returns>
		[CanBeNull]
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private static IVsBuildMacroInfo TryGetVsBuildMacroInfo([NotNull] IProjectFile file) =>
			Utils.TryGetVsHierarchy(file) as IVsBuildMacroInfo;
	}
}
