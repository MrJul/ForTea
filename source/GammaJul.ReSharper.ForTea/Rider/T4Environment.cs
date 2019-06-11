using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ReSharper.ForTea.Rider
{
	// TODO: use more accurate values
	[ShellComponent]
	public sealed class T4Environment : IT4Environment
	{
		public bool ShouldSupportOnceAttribute => true;
		public bool ShouldSupportAdvancedAttributes => true;
		public TargetFrameworkId TargetFrameworkId => TargetFrameworkId.Default;
		public CSharpLanguageLevel CSharpLanguageLevel => CSharpLanguageLevel.Latest;
		public IEnumerable<string> TextTemplatingAssemblyNames => Enumerable.Empty<string>();
		public bool IsSupported => true;
		public IEnumerable<FileSystemPath> IncludePaths => Enumerable.Empty<FileSystemPath>();
	}
}
