using System;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class ImportDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _namespaceAttribute;
		private readonly System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		public DirectiveAttributeInfo NamespaceAttribute {
			get { return _namespaceAttribute; }
		}

		public override System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		[NotNull]
		public IT4Directive CreateDirective([NotNull] string namespaceName) {
			return T4ElementFactory.Instance.CreateDirective(Name, Pair.Of(_namespaceAttribute.Name, namespaceName));
		}

		public ImportDirectiveInfo()
			: base("import") {

			_namespaceAttribute = new DirectiveAttributeInfo("namespace", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);

			_supportedAttributes = Array.AsReadOnly(new[] {
				_namespaceAttribute
			});
		}

	}

}