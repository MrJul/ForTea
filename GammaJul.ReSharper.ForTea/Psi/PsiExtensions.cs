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
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	internal static class PsiExtensions {

		[CanBeNull]
		internal static IPsiSourceFile FindSourceFileInSolution([CanBeNull] this FileSystemPath includePath, [CanBeNull] ISolution solution) {
			if (includePath == null || includePath.IsEmpty || solution == null)
				return null;

			IProjectFile includeProjectfile = solution
				.FindProjectItemsByLocation(includePath)
				.OfType<IProjectFile>()
				.FirstOrDefault();

			return includeProjectfile != null ? includeProjectfile.ToSourceFile() : null;
		}

	}

}