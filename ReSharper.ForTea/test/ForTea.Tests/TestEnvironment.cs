using System.Threading;
using GammaJul.ForTea.Core.Common;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ForTea.RiderSupport;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: RequiresThread(ApartmentState.STA)]

namespace JetBrains.ForTea.Tests
{
	[ZoneDefinition]
	public interface T4TestZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
	{
	}

	[SetUpFixture]
	public class TestEnvironment : ExtensionTestEnvironmentAssembly<T4TestZone>
	{
#pragma warning disable 169
		// These fields are here to force load assemblies
		private IT4Environment magic1; // ForTea.Core
		private T4Environment magic2; // ForTea.RiderSupport
		// private Microsoft.CodeAnalysis.MetadataReference magic3; // Roslyn
#pragma warning restore 169
	}
}
