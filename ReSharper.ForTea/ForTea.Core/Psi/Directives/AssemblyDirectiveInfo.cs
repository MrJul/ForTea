using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Directives {

	public class AssemblyDirectiveInfo : DirectiveInfo {

		[NotNull]
		public DirectiveAttributeInfo NameAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		[NotNull]
		public IT4Directive CreateDirective([NotNull] string assemblyName)
			=> T4ElementFactory.CreateDirective(Name, Pair.Of(NameAttribute.Name, assemblyName));

		public AssemblyDirectiveInfo()
			: base("assembly") {

			NameAttribute = new DirectiveAttributeInfo(
				"name",
				DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure
			);

			SupportedAttributes = ImmutableArray.FromArguments(NameAttribute);
		}

	}

}