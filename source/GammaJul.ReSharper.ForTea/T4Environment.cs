#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion


using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Application.platforms;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.VsIntegration.Shell;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using Microsoft.Win32;

namespace GammaJul.ReSharper.ForTea {

	/// <summary>
	/// Contains environment-dependent information.
	/// </summary>
	[ShellComponent]
	public class T4Environment {

		[NotNull] private readonly IVsEnvironmentInformation _vsEnvironmentInformation;
		[NotNull] private readonly Lazy<Optional<ITextTemplatingComponents>> _components;
		[NotNull] private readonly string[] _textTemplatingAssemblyNames;
		[CanBeNull] private readonly TargetFrameworkId _targetFrameworkId;
		[CanBeNull] private IList<FileSystemPath> _includePaths;

		[NotNull]
		public Optional<ITextTemplatingComponents> Components
			=> _components.Value;

		/// <summary>
		/// Gets the version of the Visual Studio we're running under, two components only, <c>Major.Minor</c>. Example: “8.0”.
		/// </summary>
		[NotNull]
		public Version2 VsVersion2
			=> _vsEnvironmentInformation.VsVersion2;

		/// <summary>
		/// Gets the target framework ID.
		/// </summary>
		[NotNull]
		public TargetFrameworkId TargetFrameworkId {
			get {
				if (_targetFrameworkId == null)
					throw CreateUnsupportedEnvironmentException();
				return _targetFrameworkId;
			}
		}

		/// <summary>
		/// Gets the C# language version.
		/// </summary>
		public CSharpLanguageLevel CSharpLanguageLevel { get; }

		/// <summary>
		/// Gets the default included assemblies.
		/// </summary>
		[NotNull]
		public IEnumerable<string> TextTemplatingAssemblyNames {
			get {
				if (_targetFrameworkId == null)
					throw CreateUnsupportedEnvironmentException();
				return _textTemplatingAssemblyNames;
			}
		}

		/// <summary>
		/// Gets whether the current environment is supported. VS2005 and VS2008 aren't.
		/// </summary>
		public bool IsSupported
			=> _targetFrameworkId != null;

		/// <summary>
		/// Gets the common include paths from the registry.
		/// </summary>
		[NotNull]
		public IEnumerable<FileSystemPath> IncludePaths {
			get {
				if (_targetFrameworkId == null)
					return EmptyList<FileSystemPath>.InstanceList;
				return _includePaths ?? (_includePaths = ReadIncludePaths());
			}
		}

		[NotNull]
		private IList<FileSystemPath> ReadIncludePaths() {
			string registryKey = _vsEnvironmentInformation.GetVisualStudioGlobalRegistryPath()
				+ @"_Config\TextTemplating\IncludeFolders\.tt";

			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey)) {

				if (key == null)
					return EmptyList<FileSystemPath>.InstanceList;

				string[] valueNames = key.GetValueNames();
				if (valueNames.Length == 0)
					return EmptyList<FileSystemPath>.InstanceList;

				var paths = new List<FileSystemPath>(valueNames.Length);
				foreach (string valueName in valueNames) {
					var value = key.GetValue(valueName) as string;
					if (String.IsNullOrEmpty(value))
						continue;

					var path = FileSystemPath.TryParse(value);
					if (!path.IsEmpty && path.IsAbsolute)
						paths.Add(path);
				}
				return paths;
			}
		}

		[NotNull]
		[Pure]
		private static NotSupportedException CreateUnsupportedEnvironmentException()
			=> new NotSupportedException("Unsupported environment.");

		[NotNull]
		private static string CreateGacAssemblyName([NotNull] string name, int majorVersion)
			=> String.Format(
				CultureInfo.InvariantCulture,
				"{0}.{1}.0, Version={1}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
				name,
				majorVersion);

		[NotNull]
		[Pure]
		private static string CreateDevEnvPublicAssemblyName([NotNull] IVsEnvironmentInformation vsEnvironmentInformation, [NotNull] string name)
			=> vsEnvironmentInformation
				.DevEnvInstallDir
				.Combine(RelativePath.Parse("PublicAssemblies\\" + name + ".dll"))
				.FullPath;

		public T4Environment([NotNull] IVsEnvironmentInformation vsEnvironmentInformation, [NotNull] RawVsServiceProvider rawVsServiceProvider) {
			_vsEnvironmentInformation = vsEnvironmentInformation;
			
			_components = Lazy.Of(() => new Optional<ITextTemplatingComponents>(rawVsServiceProvider.Value.GetService<STextTemplating, ITextTemplatingComponents>()), true);
			
			switch (vsEnvironmentInformation.VsVersion2.Major) {
				
				case VsVersions.Vs2010:
					_targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 0));
					CSharpLanguageLevel = CSharpLanguageLevel.CSharp40;
					_textTemplatingAssemblyNames = new[] {
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 10),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
					};
					break;

				case VsVersions.Vs2012:
					_targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 5));
					CSharpLanguageLevel = CSharpLanguageLevel.CSharp50;
					_textTemplatingAssemblyNames = new[] {
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 11),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 11),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
					};
					break;

				case VsVersions.Vs2013:
					_targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 5));
					CSharpLanguageLevel = CSharpLanguageLevel.CSharp50;
					_textTemplatingAssemblyNames = new[] {
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 12),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 11),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
					};
					break;

				case VsVersions.Vs2015:
					_targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 5));
					const int vs2015Update2Build = 25123;
					CSharpLanguageLevel = vsEnvironmentInformation.VsVersion4.Build >= vs2015Update2Build ? CSharpLanguageLevel.CSharp60 : CSharpLanguageLevel.CSharp50;
					_textTemplatingAssemblyNames = new[] {
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating", 14),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 11),
						CreateGacAssemblyName("Microsoft.VisualStudio.TextTemplating.Interfaces", 10)
					};
					break;

				case VsVersions.Vs2017:
					_targetFrameworkId = TargetFrameworkId.Create(FrameworkIdentifier.NetFramework, new Version(4, 6));
					CSharpLanguageLevel = CSharpLanguageLevel.CSharp70;
					_textTemplatingAssemblyNames = new[] {
						CreateDevEnvPublicAssemblyName(vsEnvironmentInformation, "Microsoft.VisualStudio.TextTemplating.15.0"),
						CreateDevEnvPublicAssemblyName(vsEnvironmentInformation, "Microsoft.VisualStudio.TextTemplating.Interfaces.11.0"),
						CreateDevEnvPublicAssemblyName(vsEnvironmentInformation, "Microsoft.VisualStudio.TextTemplating.Interfaces.10.0")
					};
					break;

				default:
					_textTemplatingAssemblyNames = EmptyArray<string>.Instance;
					break;

			}
		}

	}

}