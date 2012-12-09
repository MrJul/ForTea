using System;
using System.Collections.ObjectModel;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class IncludeDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _fileAttribute;
		private readonly ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		public DirectiveAttributeInfo FileAttribute {
			get { return _fileAttribute; }
		}

		public override ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		public IncludeDirectiveInfo()
			: base("include") {

			_fileAttribute = new DirectiveAttributeInfo("file", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);

			_supportedAttributes = Array.AsReadOnly(new[] {
				_fileAttribute
			});
		}

	}

}