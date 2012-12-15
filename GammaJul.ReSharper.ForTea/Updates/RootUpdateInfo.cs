using System;
using System.Xml.Serialization;
using JetBrains.Annotations;
using JetBrains.UI.Updates;
using JetBrains.VSIntegration.Updates;

namespace GammaJul.ReSharper.ForTea.Updates {

	[XmlRoot("RootInfo")]
	[XmlType]
	[Serializable]
	public class RootUpdateInfo {

		[NotNull]
		[XmlElement]
		[UpdatesLocalInfoManager.QueryStringContainer]
		public UpdateLocalEnvironmentInfoVs ReSharper = new UpdateLocalEnvironmentInfoVs();

		[NotNull]
		[XmlElement]
		[UpdatesLocalInfoManager.QueryStringContainer]
		public UpdateLocalEnvironmentInfo.ProductSubInfo Plugin = new UpdateLocalEnvironmentInfo.ProductSubInfo();

	}

}