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
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.TextControl;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	[NavigationProvider]
	public class T4OutsideSolutionNavigationProvider : INavigationProvider {

		private readonly ISolution _solution;
		private readonly EditorManager _editorManager;

		public IEnumerable<Type> GetSupportedTargetTypes() {
			return new[] {
				typeof(T4OutsideSolutionNavigationInfo)
			};
		}

		public IEnumerable<INavigationPoint> CreateNavigationPoints(object target, IEnumerable<INavigationPoint> basePoints) {
			var navigationInfo = target as T4OutsideSolutionNavigationInfo;
			if (navigationInfo == null)
				return basePoints;

			ITextControl textControl = _editorManager.OpenFile(navigationInfo.FileSystemPath, navigationInfo.Activate, navigationInfo.TabOptions);
			if (textControl == null)
				return basePoints;

			// the source file should exist since we just opened it
			IPsiSourceFile sourceFile = textControl.Document.GetPsiSourceFile(_solution);
			if (sourceFile == null)
				return basePoints;

			return new INavigationPoint[] {
				new TextNavigationPoint(sourceFile.ToProjectFile(), navigationInfo.TextRange.StartOffset)
			};
		}

		public double Priority {
			get { return 10000.0; }
		}

		public T4OutsideSolutionNavigationProvider([NotNull] EditorManager editorManager, [NotNull] ISolution solution) {
			_editorManager = editorManager;
			_solution = solution;
		}

	}

}