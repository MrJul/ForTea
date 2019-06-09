using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Interop.WinApi;
using JetBrains.Util.Logging;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace GammaJul.ReSharper.ForTea.VisualStudio
{
	[ShellComponent]
	public sealed class T4MacroResolver : IT4MacroResolver
	{
		public IReadOnlyDictionary<string, string> Resolve(IEnumerable<string> macros)
		{
			var result = new Dictionary<string, string>();
			IVsBuildMacroInfo vsBuildMacroInfo = null;
			foreach (string addedMacro in macros)
			{
				if (vsBuildMacroInfo == null)
				{
					vsBuildMacroInfo = TryGetVsBuildMacroInfo();
					if (vsBuildMacroInfo == null)
					{
						Logger.LogError("Couldn't get IVsBuildMacroInfo");

						break;
					}
				}

				bool succeeded =
					HResultHelpers.SUCCEEDED(vsBuildMacroInfo.GetBuildMacroValue(addedMacro, out string value))
				&& !string.IsNullOrEmpty(value);
				if (!succeeded)
				{
					value = MSBuildExtensions.GetStringValue(TryGetVsHierarchy(), addedMacro, null);
					succeeded = !string.IsNullOrEmpty(value);
				}

				if (succeeded)
					result[addedMacro] = value;
				else
					result.Remove(addedMacro);
			}

			return result;
		}
	}
}
