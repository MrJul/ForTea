using System;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class AssemblyDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _nameAttribute;
		private readonly System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		[NotNull]
		public DirectiveAttributeInfo NameAttribute {
			get { return _nameAttribute; }
		}

		public override System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		[NotNull]
		public IT4Directive CreateDirective([NotNull] string assemblyName) {
			return T4ElementFactory.Instance.CreateDirective(Name, Pair.Of(_nameAttribute.Name, assemblyName));
		}

		public AssemblyDirectiveInfo()
			: base("assembly") {

			_nameAttribute = new DirectiveAttributeInfo("name", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);

			_supportedAttributes = Array.AsReadOnly(new[] {
				_nameAttribute
			});
		}

	}

}