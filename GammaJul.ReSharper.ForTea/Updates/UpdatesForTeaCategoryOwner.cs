using System;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.PluginSupport;
using JetBrains.DataFlow;
using JetBrains.Extension;
using JetBrains.UI.Updates;
using JetBrains.VSIntegration.Updates;

namespace GammaJul.ReSharper.ForTea.Updates {

	[ShellComponent]
	public class UpdatesForTeaCategoryOwner {

		private readonly UpdatesCategory _category;

		private static void CustomizeLocalEnvironmentInfo([NotNull] OutEventArgs<object> args) {
			var reSharperInfo = args.Out as UpdateLocalEnvironmentInfoVs;
			if (reSharperInfo == null)
				args.Out = null;
			else {
				var rootInfo = new RootUpdateInfo { ReSharper = reSharperInfo };
				FillPlugInInfo(rootInfo.Plugin);
				args.Out = rootInfo;
			}
		}

		private static void FillPlugInInfo([NotNull] UpdateLocalEnvironmentInfo.ProductSubInfo info) {
			Assembly assembly = typeof(UpdatesForTeaCategoryOwner).Assembly;
			Version version = assembly.GetName().Version;
			info.CompanyName = assembly.GetCustomAttribute<PluginVendorAttribute>(false).Text;
			info.Name = assembly.GetCustomAttribute<PluginTitleAttribute>(false).Text;
			info.Version = new UpdateLocalEnvironmentInfo.VersionSubInfo(version);
			info.FullName = info.Name + " " + version.ToString(3);
		}

		public UpdatesForTeaCategoryOwner([NotNull] Lifetime lifetime, [NotNull] UpdatesManager updatesManager) {
			_category = updatesManager.Categories.AddOrActivate("ForTea", new Uri("https://raw.github.com/MrJul/ForTea/master/Updates.xslt"));
			_category.CustomizeLocalEnvironmentInfo.Advise(lifetime, CustomizeLocalEnvironmentInfo);
		}

	}

}