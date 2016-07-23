#region License

//    Copyright 2012 Julien Lebosquain
//    Copyright 2016 Caelan Sayler - [caelantsayler]at[gmail]com
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


using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Daemon.VB.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.VB.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	[DaemonStage(
		StagesBefore = new[] { typeof(GlobalFileStructureCollectorStage) },
		StagesAfter = new[] { typeof(CollectUsagesStage), typeof(VBIdentifierHighlighterStage) })]
	public class T4VBHighlightingStage : VBDaemonStageBase {

		protected override bool IsSupported(IPsiSourceFile projectFile, IContextBoundSettingsStore settingsStore) {
			return base.IsSupported(projectFile, settingsStore) && projectFile.IsLanguageSupported<T4Language>();
		}

		public override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind,
			IVBFile file) {
			return new T4VBHighlightingProcess(process, settings, file);
		}

	}

}