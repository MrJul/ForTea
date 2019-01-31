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