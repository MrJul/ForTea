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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class CultureDirectiveAttributeInfo : DirectiveAttributeInfo {
		private readonly JetBrains.Util.Lazy.Lazy<JetHashSet<string>> _cultureCodes;
		private ReadOnlyCollection<string> _intellisenseValues;

		[NotNull]
		private static JetHashSet<string> CreateCultureCodes() {
			return CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToHashSet(info => info.Name, StringComparer.OrdinalIgnoreCase);
		}

		public override bool IsValid(string value) {
			return _cultureCodes.Value.Contains(value);
		}

		public override IEnumerable<string> IntelliSenseValues {
			get { return _intellisenseValues ?? (_intellisenseValues = Array.AsReadOnly(_cultureCodes.Value.ToArray())); }
		}

		public CultureDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options) {
			_cultureCodes = Lazy.Of(CreateCultureCodes, true);
		}

	}

}