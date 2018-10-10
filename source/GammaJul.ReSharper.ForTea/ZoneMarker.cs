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

using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.ForTea {

	[ZoneMarker]
	public class ZoneMarker : IPsiLanguageZone, IRequire<ISinceVs10EnvZone>, IRequire<ILanguageCSharpZone>, IRequire<ICodeEditingZone>, IRequire<DaemonZone>, IRequire<NavigationZone> {
	}

}