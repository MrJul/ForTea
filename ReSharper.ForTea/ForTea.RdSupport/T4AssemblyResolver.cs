using System;
using System.IO;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.Diagnostics;
using JetBrains.Metadata.Reader.API;
using JetBrains.Util.Reflection;

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
			string folderPath = (info.File.ParentFolder?.Location?.FullPath).NotNull();
			string candidate = Path.Combine(folderPath, assemblyReference);

			if (File.Exists(candidate))
				return candidate;

			var assemblyNameInfo = AssemblyNameInfoFactory.Create(assemblyReference);
			var resolvedPath = CurrentRuntimeAssemblyResolvers
				.CreateInstance()
				.TryResolveAssemblyPath(assemblyNameInfo);

			if (resolvedPath?.ExistsFile == true)
				return resolvedPath.FullPath;

			// If we cannot do better, return the original file name.
			return assemblyReference;
		}

		public IDisposable Prepare(T4TemplateInfo info) => Disposable.Empty;
	}
}
