using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using System.Linq;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class EnumDirectiveAttributeInfo : DirectiveAttributeInfo {

		private readonly ReadOnlyCollection<string> _enumValues;
		
		public override bool IsValid(string value) {
			return _enumValues.Contains(value, StringComparer.OrdinalIgnoreCase);
		}

		public override IEnumerable<string> IntelliSenseValues {
			get { return _enumValues; }
		}

		public EnumDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options, [NotNull] params string[] enumValues)
			: base(name, options) {
			_enumValues = Array.AsReadOnly(enumValues);
		}

	}

}