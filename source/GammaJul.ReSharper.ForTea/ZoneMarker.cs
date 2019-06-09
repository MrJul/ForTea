using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.ForTea
{
	[ZoneMarker]
	public class ZoneMarker : IPsiLanguageZone,
		IRequire<ILanguageCSharpZone>,
		IRequire<ICodeEditingZone>,
		IRequire<DaemonZone>,
		IRequire<NavigationZone>
	{
	}
}