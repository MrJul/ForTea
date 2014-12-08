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
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.VsIntegration.Application;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.Win32;
using PlatformID = JetBrains.ProjectModel.PlatformID;

namespace GammaJul.ReSharper.ForTea {

	/// <summary>
	/// Contains environment-dependent information.
	/// </summary>
	[ShellComponent]
	public class T4Environment {

		private readonly IVsEnvironmentInformation _vsEnvironmentInformation;
		private readonly Optional<ITextTemplatingEngineHost> _textTemplatingEngineHost;
		private readonly PlatformID _platformID;
		private readonly string[] _textTemplatingAssemblyNames;
		private readonly bool _isSupported;
		private IList<FileSystemPath> _includePaths;

		[NotNull]
		public Optional<ITextTemplatingEngineHost> Host {
			get { return _textTemplatingEngineHost; }
		}

		/// <summary>
		/// Gets the version of the Visual Studio we're running under, two components only, <c>Major.Minor</c>. Example: “8.0”.
		/// </summary>
		[NotNull]
		public Version VsVersion2 {
			get { return _vsEnvironmentInformation.VsVersion2; }
		}

		/// <summary>
		/// Gets the platform ID (.NET 4.0 under VS2010, .NET 4.5 under VS2012).
		/// </summary>
		[NotNull]
		public PlatformID PlatformID {
			get {
				if (!_isSupported)
					throw new NotSupportedException("Unsupported environment.");
				return _platformID;
			}
		}

		/// <summary>
		/// Gets the default included assemblies.
		/// </summary>
		[NotNull]
		public IEnumerable<string> TextTemplatingAssemblyNames {
			get {
				if (!_isSupported)
					throw new NotSupportedException("Unsupported environment.");
				return _textTemplatingAssemblyNames;
			}
		}

		/// <summary>
		/// Gets whether the current environment is supported. VS2005 and VS2008 aren't.
		/// </summary>
		public bool IsSupported {
			get { return _isSupported; }
		}

		/// <summary>
		/// Gets the common include paths from the registry.
		/// </summary>
		[NotNull]
		public IEnumerable<FileSystemPath> IncludePaths {
			get {
				if (!_isSupported)
					return EmptyList<FileSystemPath>.InstanceList;
				return _includePaths ?? (_includePaths = ReadIncludePaths());
			}
		}

		[NotNull]
		private IList<FileSystemPath> ReadIncludePaths() {
			string registryKey = VsEnvironmentInformation.Discovery.GetVisualStudioRegistryPath(_vsEnvironmentInformation.VsHive)
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
		
		public T4Environment([NotNull] IVsEnvironmentInformation vsEnvironmentInformation, [NotNull] Optional<ITextTemplatingEngineHost> textTemplatingEngineHost) {
			_vsEnvironmentInformation = vsEnvironmentInformation;
			_textTemplatingEngineHost = textTemplatingEngineHost;

			int vsMajorVersion = vsEnvironmentInformation.VsVersion2.Major;
			switch (vsMajorVersion) {
				
				case VsVersions.Vs2010:
					_platformID = new PlatformID(FrameworkIdentifier.NetFramework, new Version(4, 0));
					_textTemplatingAssemblyNames = new[] {
						"Microsoft.VisualStudio.TextTemplating.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
						"Microsoft.VisualStudio.TextTemplating.Interfaces.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
					};
					_isSupported = true;
					break;

				case VsVersions.Vs2012:
					_platformID = new PlatformID(FrameworkIdentifier.NetFramework, new Version(4, 5));
					_textTemplatingAssemblyNames = new[] {
						"Microsoft.VisualStudio.TextTemplating.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
						"Microsoft.VisualStudio.TextTemplating.Interfaces.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
						"Microsoft.VisualStudio.TextTemplating.Interfaces.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
					};
					_isSupported = true;
					break;

				case VsVersions.Vs2013:
					_platformID = new PlatformID(FrameworkIdentifier.NetFramework, new Version(4, 5));
					_textTemplatingAssemblyNames = new[] {
						"Microsoft.VisualStudio.TextTemplating.12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
						"Microsoft.VisualStudio.TextTemplating.Interfaces.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
						"Microsoft.VisualStudio.TextTemplating.Interfaces.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
					};
					_isSupported = true;
					break;

				default:
					_isSupported = false;
					break;

			}
		}

	}

}