using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal static class VsBuildMacroHelper {

		private static readonly Regex _vsMacroRegEx = new Regex(@"\$\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		internal static void GetMacros([CanBeNull] string stringWithMacros, [NotNull] JetHashSet<string> outMacros) {
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
		internal static string ResolveMacros([NotNull] string stringWithMacros, [CanBeNull] T4PsiModule t4PsiModule) {
			if (String.IsNullOrEmpty(stringWithMacros)
			|| t4PsiModule == null
			|| stringWithMacros.IndexOf("$(", StringComparison.Ordinal) < 0)
				return stringWithMacros;

			IDictionary<string, string> macros = t4PsiModule.GetResolvedMacros();
			if (macros.Count == 0)
				return stringWithMacros;

			return _vsMacroRegEx.Replace(stringWithMacros, match => {
				Group group = match.Groups[1];
				string macro = group.Value;
				string value;
				return group.Success && macros.TryGetValue(macro, out value) ? value : macro;
			});
		}

	}

}