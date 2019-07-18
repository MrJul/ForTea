using System;
using System.IO;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;

namespace JetBrains.ForTea.RiderPlugin
{
	[ShellComponent]
	public sealed class AssemblyNamePreprocessor : IT4AssemblyNamePreprocessor
	{
		public string Preprocess(IProjectFile file, string assemblyName)
		{
			assemblyName = VsBuildMacroHelper.ResolveMacros(assemblyName, (IT4FilePsiModule) file.GetPsiModule());
			// If the argument is the fully qualified path of an existing file, then we are done.
			if (File.Exists(assemblyName)) return assemblyName;

			string folderPath = (file.ParentFolder?.Location?.FullPath).NotNull();

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

		public IDisposable Prepare(IProjectFile file) => Disposable.Empty;
	}
}
