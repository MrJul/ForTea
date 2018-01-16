#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
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

			IDictionary<string, string> macroValues = t4PsiModule.GetResolvedMacros();
			if (macroValues.Count == 0)
				return stringWithMacros;

			return ReplaceMacros(stringWithMacros, macroValues);
		}

		[NotNull]
		internal static string ResolveMacros([NotNull] string stringWithMacros, [CanBeNull] IDictionary<string, string> macroValues) {
			if (String.IsNullOrEmpty(stringWithMacros)
			|| macroValues == null
			|| macroValues.Count == 0
			|| stringWithMacros.IndexOf("$(", StringComparison.Ordinal) < 0)
				return stringWithMacros;

			return ReplaceMacros(stringWithMacros, macroValues);
		}

		[NotNull]
		private static string ReplaceMacros([NotNull] string stringWithMacros, [NotNull] IDictionary<string, string> macroValues) {
			return _vsMacroRegEx.Replace(stringWithMacros, match => {
				Group group = match.Groups[1];
				string macro = @group.Value;
				string value;
				return @group.Success && macroValues.TryGetValue(macro, out value) ? value : macro;
			});
		}

	}

}