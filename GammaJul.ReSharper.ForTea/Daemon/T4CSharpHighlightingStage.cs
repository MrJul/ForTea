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
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	[DaemonStage(StagesBefore = new[] { typeof(SmartResolverStage) }, StagesAfter = new[] { typeof(IdentifierHighlightingStage) })]
	public class T4CSharpHighlightingStage : CSharpDaemonStageBase {

		protected override bool IsSupported(IPsiSourceFile sourceFile) {
			return base.IsSupported(sourceFile) && sourceFile.IsLanguageSupported<T4Language>();
		}


		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind, ICSharpFile file) {
			return new T4CSharpHighlightingProcess(process, settings, file);
		}

	}

}