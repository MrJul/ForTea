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
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationProviders;
using JetBrains.ReSharper.Features.Navigation.Core.Navigation;
using JetBrains.TextControl;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.Navigation {

	[NavigationProvider]
	public class T4OutsideSolutionNavigationProvider : INavigationProvider<T4OutsideSolutionNavigationInfo> {

		[NotNull] private readonly ISolution _solution;
		[NotNull] private readonly IEditorManager _editorManager;
		
		public bool IsApplicable(T4OutsideSolutionNavigationInfo data)
			=> data != null;

		public IEnumerable<INavigationPoint> CreateNavigationPoints(T4OutsideSolutionNavigationInfo target)
			=> CreateNavigationPoints(target, EmptyList<INavigationPoint>.InstanceList);

		private IEnumerable<INavigationPoint> CreateNavigationPoints([NotNull] T4OutsideSolutionNavigationInfo target, [NotNull] IEnumerable<INavigationPoint> basePoints) {
			ITextControl textControl = _editorManager.OpenFile(target.FileSystemPath, new OpenFileOptions(target.Activate, tabOptions: target.TabOptions));
			if (textControl == null)
				return basePoints;

			// the source file should exist since we just opened it
			IProjectFile projectFile = textControl.Document.GetPsiSourceFile(_solution).ToProjectFile();
			if (projectFile == null)
				return basePoints;
			
			return new INavigationPoint[] {
				new TextNavigationPoint(projectFile, target.DocumentRange.StartOffset.Offset)
			};
		}

		public T4OutsideSolutionNavigationProvider([NotNull] IEditorManager editorManager, [NotNull] ISolution solution) {
			_editorManager = editorManager;
			_solution = solution;
		}

	}

}