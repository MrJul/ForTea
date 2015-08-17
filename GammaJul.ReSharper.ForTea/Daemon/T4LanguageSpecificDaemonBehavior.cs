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
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Daemon {

	[Language(typeof(T4Language))]
	public class T4LanguageSpecificDaemonBehavior : ILanguageSpecificDaemonBehavior {

		public bool CanShowErrorBox {
			get { return true; }
		}

		public bool RunInSolutionAnalysis {
			get { return true; }
		}

	    public bool RunInFindCodeIssues { get { return true; } }

	    public ErrorStripeRequest InitialErrorStripe(IPsiSourceFile sourceFile) {
			return sourceFile.Properties.ProvidesCodeModel ? ErrorStripeRequest.STRIPE_AND_ERRORS : ErrorStripeRequest.NONE;
		}

	}

}