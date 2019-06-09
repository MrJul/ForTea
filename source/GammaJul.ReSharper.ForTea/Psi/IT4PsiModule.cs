using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ReSharper.ForTea.Psi
{
	public interface IT4PsiModule : IProjectPsiModule
	{
		/// <summary>Returns the source file associated with this PSI module.</summary>
		IPsiSourceFile SourceFile { get; }

		IDictionary<string, string> GetResolvedMacros();
	}
}
