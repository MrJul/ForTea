using System;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings {

	/// <summary>Highlighting for T4 and C# that uses a Visual Studio predefined highlighter.</summary>
	[StaticSeverityHighlighting(
		Severity.INFO,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.NONE,
		ShowToolTipInStatusBar = false
	)]
	public class PredefinedHighlighting : ICustomAttributeIdHighlighting {

		private readonly DocumentRange _range;
		
		[NotNull]
		public string AttributeId { get; }

		public bool IsValid()
			=> true;

		public string ToolTip
			=> String.Empty;

		public string ErrorStripeToolTip
			=> String.Empty;

		public DocumentRange CalculateRange()
			=> _range;

		public PredefinedHighlighting([NotNull] string attributeId, DocumentRange range) {
			AttributeId = attributeId;
			_range = range;
		}

	}

}
