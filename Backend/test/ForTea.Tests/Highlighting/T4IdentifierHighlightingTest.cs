using GammaJul.ForTea.Core.Psi;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Highlighting
{
	[Ignore("Highlighting is delegated to frontend")]
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	public sealed class T4HighlightingTest : HighlightingTestBase
	{
		protected override PsiLanguageType CompilerIdsLanguage => T4Language.Instance;
		protected override string RelativeTestDataPath => @"Highlighting";

		protected override bool ColorIdentifiers => true;
		protected override bool InplaceUsageAnalysis => true;

		protected override bool HighlightingPredicate(
			IHighlighting highlighting,
			IPsiSourceFile sourceFile,
			IContextBoundSettingsStore settingsStore
		) => true;

		[TestCase("Simple")]
		[TestCase("CSharp")]
		[TestCase("VB")]
		public void TestHighlighting(string name) => DoOneTest(name);
	}
}
