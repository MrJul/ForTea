using System;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.VsIntegration.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace JetBrains.ForTea.ReSharperPlugin
{
	[ShellComponent]
	public sealed class AssemblyNamePreprocessor : IT4AssemblyNamePreprocessor
	{
		[NotNull] private readonly Lazy<Optional<ITextTemplatingComponents>> _components;

		[NotNull]
		private Optional<ITextTemplatingComponents> Components => _components.Value;

		public AssemblyNamePreprocessor([NotNull] RawVsServiceProvider provider) =>
			_components = Lazy.Of(() =>
					new Optional<ITextTemplatingComponents>(provider.Value.GetService<STextTemplating, ITextTemplatingComponents>()),
				true);

		public string Preprocess(IProjectFile file, string assemblyName) =>
			Components.CanBeNull?.Host?.ResolveAssemblyReference(assemblyName) ?? assemblyName;

		public IDisposable Prepare(IProjectFile file)
		{
			IVsHierarchy hierarchy = Utils.TryGetVsHierarchy(file);
			ITextTemplatingComponents components = Components.CanBeNull;

			if (components == null)
				return Disposable.Empty;

			object oldHierarchy = components.Hierarchy;
			string oldInputFileName = components.InputFile;

			return Disposable.CreateBracket(
				() =>
				{
					components.Hierarchy = hierarchy;
					components.InputFile = file.Location.IsNullOrEmpty() ? null : file.Location.FullPath;
				},
				() =>
				{
					components.Hierarchy = oldHierarchy;
					components.InputFile = oldInputFileName;
				},
				false
			);
		}
	}
}
