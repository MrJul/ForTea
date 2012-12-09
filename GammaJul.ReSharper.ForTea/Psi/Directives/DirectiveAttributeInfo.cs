using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class DirectiveAttributeInfo {
		private readonly string _name;
		private readonly DirectiveAttributeOptions _options;

		[NotNull]
		public string Name {
			get { return _name; }
		}

		public bool IsRequired {
			get { return (_options & DirectiveAttributeOptions.Required) == DirectiveAttributeOptions.Required; }
		}

		public bool IsDisplayedInCodeStructure {
			get { return (_options & DirectiveAttributeOptions.DisplayInCodeStructure) == DirectiveAttributeOptions.DisplayInCodeStructure; }
		}

		public virtual bool IsValid(string value) {
			return true;
		}

		[NotNull]
		public virtual IEnumerable<string> IntelliSenseValues {
			get { return EmptyList<string>.InstanceList; }
		}

		public DirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options) {
			_name = name;
			_options = options;
		}

	}

}