using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class ParameterDirectiveInfo : DirectiveInfo {

		[NotNull]
		public DirectiveAttributeInfo TypeAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo NameAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public ParameterDirectiveInfo()
			: base("parameter") {
			TypeAttribute = new DirectiveAttributeInfo("type", DirectiveAttributeOptions.Required);
			NameAttribute = new DirectiveAttributeInfo("name", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);
			SupportedAttributes = ImmutableArray.FromArguments(TypeAttribute, NameAttribute);

		}

	}

}