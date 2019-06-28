using System;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.Util;
using JetBrains.VsIntegration.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace JetBrains.ForTea.VsSupport
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

		public string Preprocess(T4TemplateInfo info, string assemblyName) =>
			Components.CanBeNull?.Host?.ResolveAssemblyReference(assemblyName) ?? assemblyName;

		public IDisposable Prepare(T4TemplateInfo info)
		{
			IVsHierarchy hierarchy = Utils.TryGetVsHierarchy(info);
			ITextTemplatingComponents components = Components.CanBeNull;

			if (components == null)
				return Disposable.Empty;

			object oldHierarchy = components.Hierarchy;
			string oldInputFileName = components.InputFile;

			return Disposable.CreateBracket(
				() =>
				{
					components.Hierarchy = hierarchy;
					components.InputFile = info.File.Location.IsNullOrEmpty() ? null : info.File.Location.FullPath;
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
