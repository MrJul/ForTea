using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.ForTea {

	[ZoneMarker]
	public class ZoneMarker : IPsiLanguageZone,
		IRequire<ISinceVs10EnvZone>,
		IRequire<ILanguageCSharpZone>,
		IRequire<ICodeEditingZone>,
		IRequire<DaemonZone>,
		IRequire<NavigationZone> {
	}

}