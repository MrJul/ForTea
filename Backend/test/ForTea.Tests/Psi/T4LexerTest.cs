using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Text;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public sealed class T4LexerTest : LexerTestBase
	{
		protected override string RelativeTestDataPath => @"Psi\Lexer";
		protected override ILexer CreateLexer(IBuffer buffer) => new T4Lexer(buffer);

		[TestCase("Simple")]
		[TestCase("CSharpCode")]
		[TestCase("VBCode")]
		public void TestLexer(string name) => DoOneTest(name);
	}
}
