using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class ImportDirectiveInfo : DirectiveInfo {

		[NotNull]
		public DirectiveAttributeInfo NamespaceAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		[NotNull]
		public IT4Directive CreateDirective([NotNull] string namespaceName)
			=> T4ElementFactory.CreateDirective(Name, Pair.Of(NamespaceAttribute.Name, namespaceName));

		public ImportDirectiveInfo()
			: base("import") {
			NamespaceAttribute = new DirectiveAttributeInfo(
				"namespace",
				DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure
			);
			SupportedAttributes = ImmutableArray.FromArguments(NamespaceAttribute);
		}

	}

}