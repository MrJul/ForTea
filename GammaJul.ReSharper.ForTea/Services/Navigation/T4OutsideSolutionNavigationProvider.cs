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

using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Navigation.Navigation;
using JetBrains.TextControl;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	[NavigationProvider]
	public class T4OutsideSolutionNavigationProvider : INavigationProvider<T4OutsideSolutionNavigationInfo> {

		private readonly ISolution _solution;
		private readonly EditorManager _editorManager;
		
		
		public bool IsApplicable(T4OutsideSolutionNavigationInfo data) {
			return data != null;
		}

		public IEnumerable<INavigationPoint> CreateNavigationPoints(T4OutsideSolutionNavigationInfo target) {
			return CreateNavigationPoints(target, EmptyList<INavigationPoint>.InstanceList);
		}

		private IEnumerable<INavigationPoint> CreateNavigationPoints([NotNull] T4OutsideSolutionNavigationInfo target, [NotNull] IEnumerable<INavigationPoint> basePoints) {
			ITextControl textControl = _editorManager.OpenFile(target.FileSystemPath, target.Activate, target.TabOptions);
			if (textControl == null)
				return basePoints;

			// the source file should exist since we just opened it
			IPsiSourceFile sourceFile = textControl.Document.GetPsiSourceFile(_solution);
			if (sourceFile == null)
				return basePoints;

			return new INavigationPoint[] {
				new TextNavigationPoint(sourceFile.ToProjectFile(), target.TextRange.StartOffset)
			};
		}

		public T4OutsideSolutionNavigationProvider([NotNull] EditorManager editorManager, [NotNull] ISolution solution) {
			_editorManager = editorManager;
			_solution = solution;
		}

	}

}