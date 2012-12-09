using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	internal sealed class T4PsiProjectFileProperties : DefaultPsiProjectFileProperties {
		private readonly bool _shouldBuildPsi;

		/// <summary>
		/// Indicates if this file should be parsed.
		/// </summary>
		public override bool ShouldBuildPsi {
			get { return _shouldBuildPsi; }
		}

		public T4PsiProjectFileProperties(IProjectFile projectFile, IPsiSourceFile sourceFile, bool shouldBuildPsi)
			: base(projectFile, sourceFile) {
			_shouldBuildPsi = shouldBuildPsi;
		}

	}

}