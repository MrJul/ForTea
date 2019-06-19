using JetBrains.Annotations;
using JetBrains.Application.changes;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	internal sealed class FakeChangeProvider : IChangeProvider
	{
		[CanBeNull] private static FakeChangeProvider instance;

		private FakeChangeProvider()
		{
		}

		public object Execute(IChangeMap changeMap) => null;

		[NotNull]
		public static FakeChangeProvider Instance => instance ?? (instance = new FakeChangeProvider());
	}
}
