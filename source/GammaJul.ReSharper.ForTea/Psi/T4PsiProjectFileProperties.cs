using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	internal sealed class T4PsiProjectFileProperties : DefaultPsiProjectFileProperties {

		/// <summary>Indicates if this file should be parsed.</summary>
		public override bool ShouldBuildPsi { get; }

		public T4PsiProjectFileProperties([NotNull] IProjectFile projectFile, [NotNull] IPsiSourceFile sourceFile, bool shouldBuildPsi)
			: base(projectFile, sourceFile) {
			ShouldBuildPsi = shouldBuildPsi;
		}

	}

}