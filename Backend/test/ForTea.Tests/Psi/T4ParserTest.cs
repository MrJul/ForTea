using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public sealed class T4ParserTest : ParserTestBase<T4Language>
	{
		protected override string RelativeTestDataPath => @"Psi\Parser";

		[TestCase("Simple")]
		[TestCase("CSharpCode")]
		[TestCase("ForgottenBlockEnd2")]
		public void TestParser(string name) => DoOneTest(name);

		[Test, Ignore("Parser cannot recover from errors well yet.")]
		public void TestForgottenBlockEnd() => DoNamedTest2();
	}
}
