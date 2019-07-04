using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests
{
	[TestFileExtension(".tt")]
	[Category("Code Completion")]
	public class T4CodeCompletionTest : CodeCompletionTestBase
	{
		protected override CodeCompletionTestType TestType => CodeCompletionTestType.List;
		protected override string RelativeTestDataPath => @"CodeCompletion";

		[Test]
		public void TestSimpleDirectiveCompletion() => DoNamedTest2();

		[Test]
		public void TestAttributeCompletion() => DoNamedTest2();

		[Test]
		public void TestAttributeValueCompletion() => DoNamedTest2();

		[Test]
		public void TestCSharpCompletion() => DoNamedTest2();
		
		[Test]
		public void TestVBNoCompletion() => DoNamedTest2();
	}
}
