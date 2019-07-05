using GammaJul.ForTea.Core.Psi;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Transformation
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public class T4ExecuteTemplateAvailabilityTest : ContextActionAvailabilityTestBase<T4ExecuteTemplateContextAction>
	{
		protected override string ExtraPath => @"TemplateProcessing";

		[Test]
		public void TestAvailability() => DoNamedTest2();
	}
}
