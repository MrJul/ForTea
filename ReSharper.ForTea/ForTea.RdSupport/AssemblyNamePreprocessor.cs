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
	public sealed class AssemblyNamePreprocessor : IT4AssemblyNamePreprocessor
	{
		// TODO: resolve macros if needed
		public string Preprocess(T4ProjectFileInfo info, string assemblyName)
		{
			// If the argument is the fully qualified path of an existing file, then we are done.
			if (File.Exists(assemblyName)) return assemblyName;

			string folderPath = (info.File.ParentFolder?.Location?.FullPath).NotNull();

			// Maybe the assembly is in the same folder as the text template that called the directive?
			string candidate = Path.Combine(folderPath, assemblyName);
			if (File.Exists(candidate)) return candidate;

			// Maybe the assembly name is missing extension?
			candidate = Path.Combine(folderPath, assemblyName + ".dll");
			if (File.Exists(candidate)) return candidate;

			// There's no need to perform other kinds of search,
			// as those will be performed by T4AssemblyReferenceManager

			// If we cannot do better, return the original file name.
			return assemblyName;
		}

		public IDisposable Prepare(T4ProjectFileInfo info) => Disposable.Empty;
	}
}
