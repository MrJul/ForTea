using System;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	[Flags]
	public enum DirectiveAttributeOptions {
		None = 0,
		Required = 1 << 0,
		DisplayInCodeStructure = 1 << 1
	}

}