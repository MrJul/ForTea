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
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>
	/// Highlighting for T4 and C# that uses a Visual Studio predefined highlighter.
	/// </summary>
	[StaticSeverityHighlighting(Severity.INFO, T4Language.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = false)]
	public class PredefinedHighlighting : ICustomAttributeIdHighlighting {

		[NotNull] private readonly string _attributeId;
		private readonly DocumentRange _range;
		
		public string AttributeId {
			get { return _attributeId; }
		}

		public bool IsValid() {
			return true;
		}

		public string ToolTip {
			get { return String.Empty; }
		}

		public string ErrorStripeToolTip {
			get { return String.Empty; }
		}

		public int NavigationOffsetPatch {
			get { return 0; }
		}

		public DocumentRange CalculateRange() {
			return _range;
		}

		public PredefinedHighlighting([NotNull] string attributeId, DocumentRange range) {
			_attributeId = attributeId;
			_range = range;
		}

	}

}