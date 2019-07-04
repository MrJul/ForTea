using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public class T4LexerTests : LexerTestBase
	{
		protected override string RelativeTestDataPath => @"Psi\Lexer";

		[TestCase("Simple")]
		[TestCase("CSharpCode")]
		[TestCase("VBCode")] // TODO: figure out why it recognizes C# tokens
		public void TestLexer(string name) => DoOneTest(name);
	}
}
