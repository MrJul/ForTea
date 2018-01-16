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


using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(Severity.ERROR, CSharpProjectFileType.Name, OverlapResolve = OverlapResolveKind.ERROR, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE)]
	public class MissingTransformTextMethodHighlighting : IHighlighting {

		private readonly IDeclaredTypeUsage _declaredTypeUsage;

		[NotNull]
		public IDeclaredTypeUsage DeclaredTypeUsage {
			get { return _declaredTypeUsage; }
		}

		public bool IsValid() {
			return _declaredTypeUsage.IsValid();
		}

		public string ToolTip {
			get { return "Base class doesn't have a valid TransformText method."; }
		}

		public string ErrorStripeToolTip {
			get { return ToolTip; }
		}

		public int NavigationOffsetPatch {
			get { return 0; }
		}

		public DocumentRange CalculateRange() {
			return _declaredTypeUsage.GetNavigationRange();
		}

		public MissingTransformTextMethodHighlighting([NotNull] IDeclaredTypeUsage declaredTypeUsage) {
			_declaredTypeUsage = declaredTypeUsage;
		}

	}

}