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
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.ForTea.Daemon {

    /// <summary>
    /// Base class for every T4 daemon stage.
    /// </summary>
    public abstract class T4DaemonStage : IDaemonStage {

	    /// <summary>
	    /// Creates a code analysis process corresponding to this stage for analysing a file.
	    /// </summary>
	    /// <returns>Code analysis process to be executed or <c>null</c> if this stage is not available for this file.</returns>
	    public IEnumerable<IDaemonStageProcess> CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind) {
		    var t4File = process.SourceFile.GetTheOnlyPsiFile(T4Language.Instance) as IT4File;
			if (t4File == null)
				return EmptyList<IDaemonStageProcess>.InstanceList;

		    return new[] { CreateProcess(process, t4File) };
	    }

	    [NotNull]
	    protected abstract IDaemonStageProcess CreateProcess([NotNull] IDaemonProcess process, [NotNull] IT4File file);

	    /// <summary>
	    /// Check the error stripe indicator necessity for this stage after processing given <paramref name="sourceFile"/>
	    /// </summary>
	    public abstract ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore);

    }

}