using System;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.Util;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal static class TextTemplatingComponentsExtensions {

		public static IDisposable With(
			[CanBeNull] this ITextTemplatingComponents components,
			[CanBeNull] IVsHierarchy hierarchy,
			[CanBeNull] FileSystemPath inputFilePath
		) {
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
				false
			);
		}

	}

}