using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public sealed class T4ErrorHighlightingTest : T4ErrorHighlightingTestBase
	{
		protected override string RelativeTestDataPath => @"Highlighting\Error";

		[TestCase("MissingClosingBraket")]
		[TestCase("CSharpUnresolvedReference")]
		public void TestHighlighting(string name) => DoOneTest(name);
	}
}
