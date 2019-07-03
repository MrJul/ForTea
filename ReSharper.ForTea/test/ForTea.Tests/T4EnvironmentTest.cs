using GammaJul.ForTea.Core.Common;
using JetBrains.Application.Components;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests
{
	[TestFixture]
	public class T4EnvironmentTest : BaseTest
	{
		[TestAttribute]
		public void TestThatT4EnvironmentSupportsEverything()
		{
			var environment = ShellInstance.GetComponent<IT4Environment>();
			Assert.NotNull(environment);
			Assert.That(environment.IsSupported);
			Assert.That(environment.ShouldSupportAdvancedAttributes);
			Assert.That(environment.ShouldSupportOnceAttribute);
		}
	}
}
