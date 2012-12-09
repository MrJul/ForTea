using System;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	[Flags]
	public enum DirectiveAttributeOptions {
		None = 0,
		Required = 1,
		DisplayInCodeStructure = 2
	}

}