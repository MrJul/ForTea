using NUnit.Framework;

namespace ForTea.Tests
{
	public class T4CSharpCodeCompletionTest : T4CSharpCodeCompletionTestBase
	{
		// TODO: wtf it fails without this line?
		protected override bool InitDataPackagesInTestFixtureSetup => false;

		[Test]
		public void Foo()
		{
		}
	}
}
