using System;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Common
{
	public interface IT4AssemblyResolver
	{
		[NotNull] string Resolve([NotNull] string assembly);

		[NotNull]
		IDisposable Prepare([NotNull] ProjectInfo info);
	}
}
