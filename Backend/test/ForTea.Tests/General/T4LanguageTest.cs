using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Psi;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.General
{
	[TestFixture]
	public class T4LanguageTests : BaseTest
	{
		[Test]
		public void LanguageIsRegistered()
		{
			Assert.NotNull(T4Language.Instance);
			Assert.NotNull(Languages.Instance.GetLanguageByName(T4Language.Name));
		}
	}
}
