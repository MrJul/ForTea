using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using JetBrains.VsIntegration.Shell;

namespace GammaJul.ReSharper.ForTea {

	[WrapVsInterfaces]
	public class ExposeTextTemplatingEngineServices : IExposeVsServices {

		public void Register(VsServiceProviderResolver.VsServiceMap map) {
			if (map.Resolve(typeof(ITextTemplatingEngineHost)) != null)
				return;

			map.QueryService<STextTemplating>().As<ITextTemplatingEngineHost>();
		}

	}

}