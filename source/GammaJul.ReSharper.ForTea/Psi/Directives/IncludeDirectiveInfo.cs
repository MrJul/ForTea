using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class IncludeDirectiveInfo : DirectiveInfo {

		[NotNull]
		public DirectiveAttributeInfo FileAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo OnceAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public IncludeDirectiveInfo([NotNull] T4Environment environment)
			: base("include") {

			FileAttribute = new DirectiveAttributeInfo("file", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);
			OnceAttribute = new BooleanDirectiveAttributeInfo("once", DirectiveAttributeOptions.None);

			SupportedAttributes = ImmutableArray.FromArguments(
				environment.VsVersion2.Major >= 12
					? new[] { FileAttribute, OnceAttribute }
					: new[] { FileAttribute }
			);
		}

	}

}