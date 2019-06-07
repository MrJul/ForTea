using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Settings;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	[SettingsKey(typeof(AutopopupEnabledSettingsKey), "T4")]
	public class T4AutopopupSettingsKey {

		[SettingsEntry(AutopopupType.HardAutopopup, "In directives")]
		public AutopopupType InDirectives;

	}

}