using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.VisualStudio
{
	[ZoneMarker]
	public class ZoneMarker : IPsiLanguageZone, IRequire<ISinceVs10EnvZone>
	{
	}
}
