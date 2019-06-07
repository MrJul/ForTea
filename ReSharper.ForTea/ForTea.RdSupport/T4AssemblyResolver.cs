using System;
using System.IO;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.Diagnostics;

namespace JetBrains.ForTea.RdSupport
{
	[ShellComponent]
	public sealed class T4AssemblyResolver : IT4AssemblyResolver
	{
		public string Resolve(T4TemplateInfo info, string assemblyReference)
		{
			// If the argument is the fully qualified path of an existing file, then we are done.
			if (File.Exists(assemblyReference))
				return assemblyReference;

			// Maybe the assembly is in the same folder as the text template that called the directive?
			string path = (info.File.ParentFolder?.Path?.ToString()).NotNull();
			string candidate = Path.Combine(path, assemblyReference);

			if (File.Exists(candidate))
				return candidate;

			// This can be customized to search specific paths for the file or to search the GAC.
			// This can be customized to accept paths to search as command line arguments.

			// If we cannot do better, return the original file name.
			return assemblyReference;
		}

		public IDisposable Prepare(T4TemplateInfo info) => Disposable.Empty;
	}
}
