using System.Threading;
using GammaJul.ForTea.Core.Common;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ForTea.RiderPlugin;
using JetBrains.ReSharper.Host.Env;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

namespace JetBrains.ForTea.Tests
{
	[ZoneDefinition]
	public interface T4TestZone : ITestsEnvZone,
		IRequire<PsiFeatureTestZone>,
		IRequire<IRiderPlatformZone>
	{
	}

	[SetUpFixture]
	public sealed class TestEnvironment : T4ExtensionTestEnvironmentAssembly<T4TestZone> // HACK
	{
#pragma warning disable 169
		// These fields are here to force load assemblies
		private IT4Environment magic1; // ForTea.Core
		private T4Environment magic2; // ForTea.RiderPlugin
#pragma warning restore 169
	}
}
