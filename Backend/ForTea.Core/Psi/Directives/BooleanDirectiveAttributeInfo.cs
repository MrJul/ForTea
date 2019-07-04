using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Directives {

	public class BooleanDirectiveAttributeInfo : EnumDirectiveAttributeInfo {
		
		public BooleanDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options, "true", "false") {
		}

	}

}