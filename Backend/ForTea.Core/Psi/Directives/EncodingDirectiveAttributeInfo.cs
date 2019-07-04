using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Directives {

	public class EncodingDirectiveAttributeInfo : DirectiveAttributeInfo {

		[NotNull] [ItemNotNull] private readonly Lazy<JetHashSet<string>> _encodings;

		[ItemNotNull] [CanBeNull] private ImmutableArray<string> _intellisenseValues;

		[NotNull]
		private static JetHashSet<string> CreateEncodings()
			=> Encoding.GetEncodings().Select(info => info.Name).ToJetHashSet(StringComparer.OrdinalIgnoreCase);

		public override bool IsValid(string value)
			=> _encodings.Value.Contains(value);

		public override ImmutableArray<string> IntelliSenseValues
			=> _intellisenseValues ?? (_intellisenseValues = _encodings.Value.ToImmutableArray());

		public EncodingDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options) {
			_encodings = Lazy.Of(CreateEncodings, true);
		}

	}

}