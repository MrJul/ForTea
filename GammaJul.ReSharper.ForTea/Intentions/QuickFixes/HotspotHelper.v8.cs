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


using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros.Implementations;
using JetBrains.ReSharper.LiveTemplates;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	internal static class HotspotHelper {

		[NotNull]
		internal static HotspotInfo CreateBasicCompletionHotspotInfo([NotNull] string fieldName, DocumentRange range) {
			return new HotspotInfo(
				new TemplateField(fieldName, new MacroCallExpressionNew(new BasicCompletionMacroDef()), 0),
				range);
		}

	}

}