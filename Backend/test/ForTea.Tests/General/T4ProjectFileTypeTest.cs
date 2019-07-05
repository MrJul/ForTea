using GammaJul.ForTea.Core.Psi;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.General
{
	[TestFixture]
	public sealed class T4ProjectFileTypeTest : BaseTest
	{
		[Test]
		public void ProjectFileTypeIsRegistered()
		{
			Assert.NotNull(T4ProjectFileType.Instance);
			var projectFileTypes = ShellInstance.GetComponent<IProjectFileTypes>();
			Assert.NotNull(projectFileTypes.GetFileType(T4ProjectFileType.Name));
		}

		[Test]
		public void ProjectFileTypeFromExtension()
		{
			var projectFileExtensions = ShellInstance.GetComponent<IProjectFileExtensions>();
			Assert.AreSame(T4ProjectFileType.Instance, projectFileExtensions.GetFileType(T4ProjectFileType.MainExtension));
		}
	}
}
