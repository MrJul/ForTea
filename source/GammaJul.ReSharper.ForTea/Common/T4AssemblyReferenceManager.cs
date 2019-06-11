using System;
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea
{
	public sealed class T4AssemblyReferenceManager : IDisposable
	{
		[CanBeNull] private IModuleReferenceResolveManager _resolveManager;
		[NotNull] private readonly IAssemblyFactory _assemblyFactory;
		[NotNull] private readonly T4TemplateInfo _t4TemplateInfo;
		[NotNull] private readonly T4ResolveProject _resolveProject;

		[NotNull]
		public IModuleReferenceResolveContext ModuleReferenceResolveContext { get; }

		[NotNull]
		public Dictionary<string, IAssemblyCookie> References { get; } =
			new Dictionary<string, IAssemblyCookie>(StringComparer.OrdinalIgnoreCase);

		/// <summary>Gets an instance of <see cref="IModuleReferenceResolveManager"/> sed to resolve assemblies.</summary>
		[NotNull]
		private IModuleReferenceResolveManager ResolveManager
			=> _resolveManager ?? (_resolveManager = _t4TemplateInfo.Solution.GetComponent<IModuleReferenceResolveManager>());

		internal T4AssemblyReferenceManager(
			[NotNull] IAssemblyFactory assemblyFactory,
			[NotNull] T4TemplateInfo t4TemplateInfo,
			[NotNull] T4ResolveProject resolveProject,
			[NotNull] IModuleReferenceResolveContext moduleReferenceResolveContext
		)
		{
			_assemblyFactory = assemblyFactory;
			_t4TemplateInfo = t4TemplateInfo;
			_resolveProject = resolveProject;
			ModuleReferenceResolveContext = moduleReferenceResolveContext;
		}

		/// <summary>Try to add an assembly reference to the list of assemblies.</summary>
		/// <param name="assemblyNameOrFile"></param>
		/// <remarks>Does not refresh references, simply add a cookie to the cookies list.</remarks>
		[CanBeNull]
		public IAssemblyCookie TryAddReference([NotNull] string assemblyNameOrFile)
		{
			var cookie = CreateCookie(assemblyNameOrFile);
			if (cookie != null)
				References.Add(assemblyNameOrFile, cookie);

			return cookie;
		}

		/// <summary>Creates a new <see cref="IAssemblyCookie"/> from an assembly full name.</summary>
		/// <param name="assemblyNameOrFile">The assembly full name.</param>
		/// <returns>An instance of <see cref="IAssemblyCookie"/>, or <c>null</c> if none could be created.</returns>
		[CanBeNull]
		private IAssemblyCookie CreateCookie(string assemblyNameOrFile)
		{
			if (assemblyNameOrFile == null)
				return null;

			assemblyNameOrFile = assemblyNameOrFile.Trim();

			if (assemblyNameOrFile.Length == 0)
				return null;

			AssemblyReferenceTarget target = null;

			// assembly path
			FileSystemPath path = FileSystemPath.TryParse(assemblyNameOrFile);
			if (!path.IsEmpty && path.IsAbsolute)
				target = new AssemblyReferenceTarget(AssemblyNameInfo.Empty, path);

			// assembly name
			else
			{
				AssemblyNameInfo nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
				if (nameInfo != null)
					target = new AssemblyReferenceTarget(nameInfo, FileSystemPath.Empty);
			}

			if (target == null)
				return null;

			return CreateCookieCore(target);
		}

		[CanBeNull]
		private IAssemblyCookie CreateCookieCore([NotNull] AssemblyReferenceTarget target)
		{
			FileSystemPath result = ResolveManager.Resolve(target, _resolveProject, ModuleReferenceResolveContext);

			return result != null
				? _assemblyFactory.AddRef(result, "T4", ModuleReferenceResolveContext)
				: null;
		}

		public void Dispose() =>
			_resolveProject.Dispose();
	}
}
