using System;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Common
{
	public interface IT4AssemblyResolver
	{
		[NotNull] string Resolve([NotNull] T4TemplateInfo info, [NotNull] string assembly);

		[NotNull]
		IDisposable Prepare([NotNull] T4TemplateInfo info);
	}
}
