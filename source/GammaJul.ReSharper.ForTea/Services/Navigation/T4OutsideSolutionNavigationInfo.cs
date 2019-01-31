using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	public class T4OutsideSolutionNavigationInfo {

		[NotNull]
		public FileSystemPath FileSystemPath { get; }

		public DocumentRange DocumentRange { get; }

		public bool Activate { get; }

		public TabOptions TabOptions { get; }

		public T4OutsideSolutionNavigationInfo(
			[NotNull] FileSystemPath fileSystemPath,
			DocumentRange documentRange,
			bool activate,
			TabOptions tabOptions
		) {
			FileSystemPath = fileSystemPath;
			DocumentRange = documentRange;
			Activate = activate;
			TabOptions = tabOptions;
		}

	}

}