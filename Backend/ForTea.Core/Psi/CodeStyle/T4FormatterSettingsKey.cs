using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.EditorConfig;
using JetBrains.ReSharper.Psi.Format;

namespace GammaJul.ForTea.Core.Psi.CodeStyle
{
	[SettingsKey(typeof(CSharpFormatSettingsKey), "Code formatting in T4 C#")]
	[EditorConfigKey("t4")]
	public class T4FormatterSettingsKey : FormatSettingsKeyBase
	{
	}
}
