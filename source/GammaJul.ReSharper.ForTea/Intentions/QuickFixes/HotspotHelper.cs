using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros.Implementations;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Templates;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	internal static class HotspotHelper {

		[NotNull]
		public static HotspotInfo CreateBasicCompletionHotspotInfo([NotNull] string fieldName, DocumentRange range)
			=> new HotspotInfo(
				new TemplateField(fieldName, new MacroCallExpressionNew(new BasicCompletionMacroDef()), 0),
				range
			);

	}

}