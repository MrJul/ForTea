using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Services {

	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4SpecificOwnerUtil : DefaultFileTypeSpecificOwnerUtil {

		public override bool CanContainSeveralClasses(IPsiSourceFile sourceFile) {
			return false;
		}

		public override bool CanImplementInterfaces(ITypeDeclaration typeElement) {
			return !typeElement.IsSynthetic();
		}

		public override bool CanHaveConstructors(ITypeDeclaration typeElement) {
			return !typeElement.IsSynthetic();
		}

		public override bool SuperClassCanBeChanged(ITypeDeclaration typeElement) {
			// TODO: handle template inherits attribute
			return !typeElement.IsSynthetic();
		}

		public override bool HasUglyName(ITypeDeclaration declaration) {
			return declaration.IsSynthetic();
		}

	}

}