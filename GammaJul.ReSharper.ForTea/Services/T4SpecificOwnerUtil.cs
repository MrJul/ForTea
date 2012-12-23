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