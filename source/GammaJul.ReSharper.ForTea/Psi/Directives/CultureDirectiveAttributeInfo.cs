using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class CultureDirectiveAttributeInfo : DirectiveAttributeInfo {

		[NotNull] [ItemNotNull] private readonly Lazy<JetHashSet<string>> _cultureCodes;
		[CanBeNull] [ItemNotNull] private ImmutableArray<string> _intellisenseValues;

		[NotNull]
		private static JetHashSet<string> CreateCultureCodes()
			=> CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToJetHashSet(info => info.Name, StringComparer.OrdinalIgnoreCase);

		public override bool IsValid(string value)
			=> _cultureCodes.Value.Contains(value);

		public override ImmutableArray<string> IntelliSenseValues
			=> _intellisenseValues ?? (_intellisenseValues = _cultureCodes.Value.ToImmutableArray());

		public CultureDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options) {
			_cultureCodes = Lazy.Of(CreateCultureCodes, true);
		}

	}

}