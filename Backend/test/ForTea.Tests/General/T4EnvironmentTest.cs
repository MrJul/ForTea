using GammaJul.ForTea.Core.Common;
using JetBrains.Application.Components;
using JetBrains.Application.StdApplicationUI.TaskBar;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.General
{
	[TestFixture]
	public class T4EnvironmentTest : BaseTest
	{
		[Test]
		public void TestThatT4EnvironmentSupportsEverything()
		{
			var environment = ShellInstance.GetComponent<IT4Environment>();
			Assert.NotNull(environment);
			Assert.That(environment.IsSupported);
			Assert.That(environment.ShouldSupportAdvancedAttributes);
			Assert.That(environment.ShouldSupportOnceAttribute);
		}

		[Test]
		public void TestThatSomePlatformShellComponentIsAccessible() =>
			Assert.NotNull(ShellInstance.GetComponent<ITaskBarManager>());
	}
}
