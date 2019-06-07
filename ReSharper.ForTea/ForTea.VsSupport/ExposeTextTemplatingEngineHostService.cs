using JetBrains.VsIntegration.Shell;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace JetBrains.ForTea.VsSupport {

	[WrapVsInterfaces]
	public class ExposeTextTemplatingEngineServices : IExposeVsServices {

		public void Register(VsServiceProviderResolver.VsServiceMap map) {
			if (map.Resolve(typeof(ITextTemplatingEngineHost)) != null)
				return;

			map.QueryService<STextTemplating>().As<ITextTemplatingEngineHost>();
		}

	}

}