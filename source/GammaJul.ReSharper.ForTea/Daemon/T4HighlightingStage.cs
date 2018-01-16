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
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Daemon stage that creates processes for highlighting tokens.
	/// </summary>
	[DaemonStage(
		StagesBefore = new[] { typeof(GlobalFileStructureCollectorStage) },
		StagesAfter = new[] { typeof(CollectUsagesStage), typeof(IdentifierHighlightingStage) })]
	public class T4HighlightingStage : T4DaemonStage {

		protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IT4File file) {
			return new T4HighlightingProcess(file, process);
		}

		/// <summary>
		/// Check the error stripe indicator necessity for this stage after processing given <paramref name="sourceFile"/>
		/// </summary>
		public override ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore) {
			return ErrorStripeRequest.NONE;
		}

	}

}