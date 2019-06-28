using System;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Common
{
	public interface IT4AssemblyNamePreprocessor
	{
		[NotNull]
		string Preprocess([NotNull] T4TemplateInfo info, [NotNull] string assemblyName);

		[NotNull]
		IDisposable Prepare([NotNull] T4TemplateInfo info);
	}
}
