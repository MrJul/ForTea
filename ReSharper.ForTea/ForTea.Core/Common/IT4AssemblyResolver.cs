using System;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Common
{
	public interface IT4AssemblyResolver
	{
		[NotNull] string Resolve([NotNull] T4TemplateInfo info, [NotNull] string assembly);

		[NotNull]
		IDisposable Prepare([NotNull] T4TemplateInfo info);
	}
}
