using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	[Category("Code Completion")]
	public class T4CodeCompletionTest : CodeCompletionTestBase
	{
		protected override CodeCompletionTestType TestType => CodeCompletionTestType.List;
		protected override string RelativeTestDataPath => @"CodeCompletion";

		[TestCase("Directive")]
		[TestCase("Attribute")]
		[TestCase("AttributeValue")]
		[TestCase("CSharp")]
		[TestCase("VB")]
		public void TestCompletion(string name) => DoOneTest(name);
	}
}
