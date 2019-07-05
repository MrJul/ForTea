using GammaJul.ForTea.Core.Psi;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Actions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Transformation
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public sealed class T4ExecuteTemplateTest : ContextActionExecuteTestBase<T4ExecuteTemplateContextAction>
	{
		protected override string ExtraPath => @"TemplateProcessing";

		[TestCase("PlainText")]
		[TestCase("DefaultExtension")]
		[TestCase("DefaultDirectives")]
		[TestCase("PushIndent")]
		[TestCase("NewlinesInFeature")]
		[TestCase("TextInFeature")]
		[TestCase("ExtensionWithoutDot")]
		[TestCase("LineBreakMess")]
		public void TestExecuteTemplate(string name) => DoOneTest(name);
	}
}
