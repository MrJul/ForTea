using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class BooleanDirectiveAttributeInfo : EnumDirectiveAttributeInfo {
		
		public BooleanDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options, "true", "false") {
		}

	}

}