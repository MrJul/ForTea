using GammaJul.ReSharper.ForTea.Parsing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Language service for T4 project files.
	/// </summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public sealed class T4ProjectFileLanguageService : MixedProjectFileLanguageService {
		private readonly T4Environment _t4Environment;

		/// <summary>
		/// Gets the PSI language type, <see cref="T4Language"/>.
		/// </summary>
		protected override PsiLanguageType PsiLanguageType {
			get { return T4Language.Instance; }
		}

		/// <summary>
		/// Gets the T4 file icon.
		/// TODO: add an image.
		/// </summary>
		public override IconId Icon {
			get { return null; }
		}

		/// <summary>
		/// Get the PSI properties (if any) for the specific project file
		/// </summary>
		public override IPsiSourceFileProperties GetPsiProperties(IProjectFile projectFile, IPsiSourceFile sourceFile) {
			return new T4PsiProjectFileProperties(projectFile, sourceFile, _t4Environment.IsSupported);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4ProjectFileLanguageService"/> class.
		/// </summary>
		/// <param name="t4ProjectFileType">Type of the T4 project file.</param>
		/// <param name="t4Environment">The host environment.</param>
		public T4ProjectFileLanguageService(T4ProjectFileType t4ProjectFileType, T4Environment t4Environment)
			: base(t4ProjectFileType) {
			_t4Environment = t4Environment;
		}

	}

}
