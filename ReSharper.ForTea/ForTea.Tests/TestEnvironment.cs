using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ForTea.RiderSupport;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

#pragma warning disable 169

namespace ForTea.Tests
{
	[ZoneDefinition]
	public interface T4TestZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
	{
	}

	[SetUpFixture]
	public class TestEnvironment : ExtensionTestEnvironmentAssembly<T4TestZone>
	{
		// I'll reference here types from all the assemblies I use. Just in case.
		// ReSharper disable once RedundantNameQualifier
		private GammaJul.ForTea.Core.Common.IT4Environment fieldOfTypeFromForTea_Core;
		// ReSharper disable once RedundantNameQualifier
		private T4Environment fieldOfTypeFromForTea_RiderSupport;
	}
}
#pragma warning restore 169
