using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ForTea.Core.Psi.Directives {

	public class OutputDirectiveInfo : DirectiveInfo {

		[NotNull]
		public DirectiveAttributeInfo ExtensionAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo EncodingAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public OutputDirectiveInfo()
			: base("output") {
			ExtensionAttribute = new DirectiveAttributeInfo("extension", DirectiveAttributeOptions.None);
			EncodingAttribute = new EncodingDirectiveAttributeInfo("encoding", DirectiveAttributeOptions.None);
			SupportedAttributes = ImmutableArray.FromArguments(ExtensionAttribute, EncodingAttribute);
		}

	}

}