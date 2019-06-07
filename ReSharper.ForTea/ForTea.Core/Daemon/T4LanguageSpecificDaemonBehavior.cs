using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon {

	[Language(typeof(T4Language))]
	public class T4LanguageSpecificDaemonBehavior : ILanguageSpecificDaemonBehavior {

		public bool CanShowErrorBox
			=> true;

		public bool RunInSolutionAnalysis
			=> true;

		public bool RunInFindCodeIssues
			=> true;

		public ErrorStripeRequest InitialErrorStripe(IPsiSourceFile sourceFile)
			=> sourceFile.Properties.ProvidesCodeModel ? ErrorStripeRequest.STRIPE_AND_ERRORS : ErrorStripeRequest.NONE;

	}

}