using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Interop.WinApi;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.VsSupport
{
	[ShellComponent]
	public sealed class T4MacroResolver : T4MacroResolverBase
	{
		public T4MacroResolver(IT4AssemblyNamePreprocessor preprocessor) : base(preprocessor)
		{
		}

		public override IReadOnlyDictionary<string, string> Resolve(
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

		/// <summary>
		/// The <see cref="IVsHierarchy"/> representing the project file
		/// normally implements <see cref="IVsBuildMacroInfo"/>.
		/// </summary>
		/// <returns>An instance of <see cref="IVsBuildMacroInfo"/> if found.</returns>
		[CanBeNull]
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private static IVsBuildMacroInfo TryGetVsBuildMacroInfo([NotNull] T4TemplateInfo info) =>
			Utils.TryGetVsHierarchy(info) as IVsBuildMacroInfo;
	}
}
