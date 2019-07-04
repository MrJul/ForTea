using JetBrains.Application.changes;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	internal sealed class FakeChangeProvider : IChangeProvider
	{
		public object Execute(IChangeMap changeMap) => null;
	}
}
