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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.Util;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal static class TextTemplatingComponentsExtensions {

		[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
		public static IDisposable With([CanBeNull] this ITextTemplatingComponents components, [CanBeNull] IVsHierarchy hierarchy, [CanBeNull] FileSystemPath inputFilePath) {
			if (components == null)
				return Disposable.Empty;

			object oldHierarchy = components.Hierarchy;
			string oldInputFileName = components.InputFile;

			return Disposable.CreateBracket(
				() => {
					components.Hierarchy = hierarchy;
					components.InputFile = inputFilePath.IsNullOrEmpty() ? null : inputFilePath.FullPath;
				},
				() => {
					components.Hierarchy = oldHierarchy;
					components.InputFile = oldInputFileName;
				},
				false);
		}

	}

}