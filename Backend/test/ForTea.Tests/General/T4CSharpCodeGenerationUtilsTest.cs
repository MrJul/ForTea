using GammaJul.ForTea.Core.TemplateProcessing;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.General
{
	[TestFixture]
	public sealed class T4CSharpCodeGenerationUtilsTest
	{
		[Test, Sequential]
		public void TestChangeExtensionOnNameWithTwoDots(
			[Values("Name.tt", "Foo.tt")] string name,
			[Values("cs", ".tx.t")] string extension,
			[Values("Name.cs", "Foo..tx.t")] string expected
		) => Assert.AreEqual(expected, name.WithOtherExtension(extension));
	}
}
