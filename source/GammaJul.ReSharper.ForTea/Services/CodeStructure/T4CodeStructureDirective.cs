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
using System.IO;
using System.Text;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.UI.Controls.TreeView;
using JetBrains.Application.UI.Controls.Utils;
using JetBrains.Application.UI.TreeModels;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	internal sealed class T4CodeStructureDirective : T4CodeStructureElement<IT4Directive> {

		private readonly DirectiveInfoManager _directiveInfoManager;

		[NotNull]
		private string GetDirectiveText() {
			IT4Directive directive = GetTreeNode();
			string name = directive != null ? directive.GetName() : null;
			if (name == null)
				return "???";

			DirectiveInfo directiveInfo = _directiveInfoManager.GetDirectiveByName(name);
			if (directiveInfo == null)
				return name;

			// display the directive with the attributes that are marked with DisplayInCodeStructure
			var builder = new StringBuilder(name);
			foreach (IT4DirectiveAttribute attribute in directive.GetAttributes()) {
				DirectiveAttributeInfo attributeInfo = directiveInfo.GetAttributeByName(attribute.GetName());
				if (attributeInfo == null || !attributeInfo.IsDisplayedInCodeStructure)
					continue;

				builder.Append(' ');
				builder.Append(attributeInfo.Name);
				builder.Append("=\"");
				builder.Append(attribute.GetValue());
				builder.Append('"');
			}
			return builder.ToString();
		}

		public override void Present(StructuredPresenter<TreeModelNode, IPresentableItem> presenter, IPresentableItem item, TreeModelNode modelNode, PresentationState state) {
			item.RichText = GetDirectiveText();
			item.Images.Add(PsiWebThemedIcons.AspDirective.Id);
		}

		protected override void DumpSelf(TextWriter builder) {
			builder.Write(GetDirectiveText());
		}

		public override DocumentRange NavigationRange {
			get {
				IT4Directive directive = GetTreeNode();
				if (directive == null)
					return DocumentRange.InvalidRange;

				IT4Token nameToken = directive.GetNameToken();
				if (nameToken == null)
					return directive.GetNavigationRange();

				return nameToken.GetNavigationRange();
			}
		}

		public override IList<string> GetQuickSearchTexts() {
			IT4Directive directive = GetTreeNode();
			string name = directive != null ? directive.GetName() : null;
			return name != null ? new[] { name } : EmptyList<string>.InstanceList;
		}

		public T4CodeStructureDirective([NotNull] CodeStructureElement parent, [NotNull] IT4Directive directive,
			[NotNull] DirectiveInfoManager directiveInfoManager)
			: base(parent, directive) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}