using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Common
{
	public interface IT4Environment
	{
		/// <summary>
		/// Indicates whether plugin should support 'once' attribute in 'include' directive.
		/// </summary>
		// <#@ include file="foo" once="true" #>
		bool ShouldSupportOnceAttribute { get; }

		/// <summary>
		/// Indicates whether plugin should support 'linePragmas' and 'visibility' attributes
		/// </summary>
		bool ShouldSupportAdvancedAttributes { get; }

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
	}
}
