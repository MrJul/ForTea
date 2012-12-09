using JetBrains.Annotations;
using JetBrains.IDE;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	public class T4OutsideSolutionNavigationInfo {

		private readonly FileSystemPath _fileSystemPath;
		private readonly TextRange _textRange;
		private readonly bool _activate;
		private readonly TabOptions _tabOptions;

		[NotNull]
		public FileSystemPath FileSystemPath {
			get { return _fileSystemPath; }
		}

		public TextRange TextRange {
			get { return _textRange; }
		}

		public bool Activate {
			get { return _activate; }
		}

		public TabOptions TabOptions {
			get { return _tabOptions; }
		}

		public T4OutsideSolutionNavigationInfo([NotNull] FileSystemPath fileSystemPath, TextRange textRange, bool activate, TabOptions tabOptions) {
			_fileSystemPath = fileSystemPath;
			_textRange = textRange;
			_activate = activate;
			_tabOptions = tabOptions;
		}

	}

}