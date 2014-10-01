using JetBrains.VsIntegration.Application;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace GammaJul.ReSharper.ForTea {

	[WrapVsInterfaces]
	public class ExposeTextTemplatingEngineServices : IExposeVsServices {

		public void Register(VsServiceProviderComponentContainer.VsServiceMap map) {
			if (map.Resolve(typeof(ITextTemplatingEngineHost)) != null)
				return;

			map.QueryService<STextTemplating>().As<ITextTemplatingEngineHost>();
		}

	}

}