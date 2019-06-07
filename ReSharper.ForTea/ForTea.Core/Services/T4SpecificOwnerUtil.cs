using GammaJul.ForTea.Core.Psi;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Services {

	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4SpecificOwnerUtil : DefaultFileTypeSpecificOwnerUtil {

		public override bool CanContainSeveralClasses(IPsiSourceFile sourceFile)
			=> false;

		public override bool CanImplementInterfaces(ITypeDeclaration typeElement)
			=> !typeElement.IsSynthetic();

		public override bool CanHaveConstructors(ITypeDeclaration typeElement)
			=> !typeElement.IsSynthetic();

		public override bool HasUglyName(ITypeDeclaration declaration)
			=> declaration.IsSynthetic();

	}

}