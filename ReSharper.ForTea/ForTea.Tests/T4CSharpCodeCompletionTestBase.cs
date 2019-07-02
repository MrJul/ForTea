//using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace ForTea.Tests
{
	[Category("T4"), Category("Code Completion")]
	[TestFileExtension(".tt")]
	public abstract class T4CSharpCodeCompletionTestBase : CodeCompletionTestBase
	{
		protected override string RelativeTestDataPath => @"CodeCompletion";
		protected override CodeCompletionTestType TestType => CodeCompletionTestType.Action;
	}
}
