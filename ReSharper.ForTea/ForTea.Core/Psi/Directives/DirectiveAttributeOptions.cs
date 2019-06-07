using System;

namespace GammaJul.ForTea.Core.Psi.Directives {

	[Flags]
	public enum DirectiveAttributeOptions {
		None = 0,
		Required = 1 << 0,
		DisplayInCodeStructure = 1 << 1
	}

}