using System;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>
	/// Highlighting for T4 and C# that uses Visual Studio a predefined highlighter.
	/// </summary>
	[StaticSeverityHighlighting(Severity.INFO, T4Language.Name, OverlapResolve = OverlapResolveKind.NONE, ShowToolTipInStatusBar = false)]
	public class PredefinedHighlighting : ICustomAttributeIdHighlighting {

		private readonly string _attributeId;

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

		public PredefinedHighlighting([NotNull] string attributeId) {
			_attributeId = attributeId;
		}

	}

}