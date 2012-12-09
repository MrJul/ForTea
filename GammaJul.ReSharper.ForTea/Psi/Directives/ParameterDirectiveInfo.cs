using System;
using System.Collections.ObjectModel;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class ParameterDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _typeAttribute;
		private readonly DirectiveAttributeInfo _nameAttribute;
		private readonly ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		public DirectiveAttributeInfo TypeAttribute {
			get { return _typeAttribute; }
		}

		public DirectiveAttributeInfo NameAttribute {
			get { return _nameAttribute; }
		}

		public override ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		public ParameterDirectiveInfo()
			: base("parameter") {

			_typeAttribute = new DirectiveAttributeInfo("type", DirectiveAttributeOptions.Required);
			_nameAttribute = new DirectiveAttributeInfo("name", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);

			_supportedAttributes = Array.AsReadOnly(new[] {
				_typeAttribute,
				_nameAttribute
			});

		}

	}

}