using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class CultureDirectiveAttributeInfo : DirectiveAttributeInfo {
		private readonly Lazy<JetHashSet<string>> _cultureCodes;
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