using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.Format;

namespace GammaJul.ForTea.Core.Psi.CodeStyle {

	[SettingsKey(typeof(CodeFormattingSettingsKey), "Code formatting in T4")]
	public class T4FormatSettingsKey : FormatSettingsKeyBase {
	}

}