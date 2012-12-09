using System;
using System.Collections.ObjectModel;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class OutputDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _extensionAttribute;
		private readonly DirectiveAttributeInfo _encodingAttribute;
		private readonly ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		public DirectiveAttributeInfo ExtensionAttribute {
			get { return _extensionAttribute; }
		}

		public DirectiveAttributeInfo EncodingAttribute {
			get { return _encodingAttribute; }
		}

		public override ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		public OutputDirectiveInfo()
			: base("output") {

			_extensionAttribute = new DirectiveAttributeInfo("extension", DirectiveAttributeOptions.None);
			_encodingAttribute = new EncodingDirectiveAttributeInfo("encoding", DirectiveAttributeOptions.None);

			_supportedAttributes = Array.AsReadOnly(new[] {
				_extensionAttribute,
				_encodingAttribute
			});
		}

	}

}