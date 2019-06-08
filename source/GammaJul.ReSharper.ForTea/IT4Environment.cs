using System.Collections.Generic;
using JetBrains.Application.Components;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace GammaJul.ReSharper.ForTea
{
	public interface IT4Environment
	{
		Optional<ITextTemplatingComponents> Components { get; }

		/// <summary>
		/// Gets the version of the Visual Studio we're running under,
		/// two components only, <c>Major.Minor</c>. Example: “8.0”.
		/// </summary>
		Version2 VsVersion2 { get; }

		/// <summary>Gets the target framework ID.</summary>
		TargetFrameworkId TargetFrameworkId { get; }

		/// <summary>Gets the C# language version.</summary>
		CSharpLanguageLevel CSharpLanguageLevel { get; }

		/// <summary>Gets the default included assemblies.</summary>
		IEnumerable<string> TextTemplatingAssemblyNames { get; }

		/// <summary>Gets whether the current environment is supported. VS2005 and VS2008 aren't.</summary>
		bool IsSupported { get; }

		/// <summary>Gets the common include paths from the registry.</summary>
		IEnumerable<FileSystemPath> IncludePaths { get; }

		IList<FileSystemPath> ReadIncludePaths();
	}
}