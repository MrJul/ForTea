using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class EncodingDirectiveAttributeInfo : DirectiveAttributeInfo {
		private readonly Lazy<JetHashSet<string>> _encodings;
		private ReadOnlyCollection<string> _intellisenseValues;

		[NotNull]
		private static JetHashSet<string> CreateEncodings() {
			return new JetHashSet<string>(Encoding.GetEncodings().Select(info => info.Name), StringComparer.OrdinalIgnoreCase);
		}

		public override bool IsValid(string value) {
			return _encodings.Value.Contains(value);
		}

		public override IEnumerable<string> IntelliSenseValues {
			get { return _intellisenseValues ?? (_intellisenseValues = Array.AsReadOnly(_encodings.Value.ToArray())); }
		}

		public EncodingDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options) {
			_encodings = Lazy.Of(CreateEncodings, true);
		}

	}

}