using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GammaJul.ForTea.Core.Psi.Modules;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi {

	internal static class VsBuildMacroHelper {

		[NotNull] private static readonly Regex _vsMacroRegEx = new Regex(@"\$\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		public static void GetMacros([CanBeNull] string stringWithMacros, [NotNull] JetHashSet<string> outMacros) {
			if (String.IsNullOrEmpty(stringWithMacros)
			|| stringWithMacros.IndexOf("$(", StringComparison.Ordinal) < 0)
				return;

			MatchCollection matches = _vsMacroRegEx.Matches(stringWithMacros);
			foreach (Match match in matches) {
				if (match.Success)
					outMacros.Add(match.Groups[1].Value);
			}
		}

		[NotNull]
		public static string ResolveMacros([NotNull] string stringWithMacros, [CanBeNull] IT4FilePsiModule t4FilePsiModule) {
			if (String.IsNullOrEmpty(stringWithMacros)
			|| t4FilePsiModule == null
			|| stringWithMacros.IndexOf("$(", StringComparison.Ordinal) < 0)
				return stringWithMacros;

			IDictionary<string, string> macroValues = t4FilePsiModule.GetResolvedMacros();
			if (macroValues.Count == 0)
				return stringWithMacros;

			return ReplaceMacros(stringWithMacros, macroValues);
		}

		[NotNull]
		public static string ResolveMacros([NotNull] string stringWithMacros, [CanBeNull] IDictionary<string, string> macroValues) {
			if (String.IsNullOrEmpty(stringWithMacros)
			|| macroValues == null
			|| macroValues.Count == 0
			|| stringWithMacros.IndexOf("$(", StringComparison.Ordinal) < 0)
				return stringWithMacros;

			return ReplaceMacros(stringWithMacros, macroValues);
		}

		[NotNull]
		public static string ReplaceMacros([NotNull] string stringWithMacros, [NotNull] IDictionary<string, string> macroValues)
			=> _vsMacroRegEx.Replace(stringWithMacros, match => {
				Group group = match.Groups[1];
				string macro = group.Value;
				return group.Success && macroValues.TryGetValue(macro, out string value) ? value : macro;
			});

	}

}