using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems.Impl;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	[Language(typeof(T4Language))]
	public class DirectiveAttributeNameItemsProvider : ItemsProviderOfSpecificContext<T4CodeCompletionContext> {

		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;

		protected override LookupFocusBehaviour GetLookupFocusBehaviour(T4CodeCompletionContext context)
			=> LookupFocusBehaviour.SoftWhenEmpty;

		protected override bool IsAvailable(T4CodeCompletionContext context) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			if (node == null)
				return false;

			if (node.Parent is IT4DirectiveAttribute)
				return node.GetTokenType() == T4TokenNodeTypes.Name;

			return node.GetTokenType() == T4TokenNodeTypes.Space && node.Parent is IT4Directive;
		}

		protected override bool AddLookupItems(T4CodeCompletionContext context, IItemsCollector collector) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			Assertion.AssertNotNull(node, "node == null");
			var ranges = context.BasicContext.GetRanges(node);
			collector.AddRanges(ranges);

			var directive = node.GetContainingNode<IT4Directive>();
			Assertion.AssertNotNull(directive, "directive != null");
			DirectiveInfo directiveInfo = _directiveInfoManager.GetDirectiveByName(directive.GetName());
			if (directiveInfo == null)
				return false;

			JetHashSet<string> existingNames = directive
				.GetAttributes()
				.Select(attr => attr.GetName())
				.ToJetHashSet(s => s, StringComparer.OrdinalIgnoreCase);

			foreach (string attributeName in directiveInfo.SupportedAttributes.Select(attr => attr.Name)) {
				if (existingNames.Contains(attributeName))
					continue;
				
				var item = new TextLookupItem(attributeName);
				item.InitializeRanges(ranges, context.BasicContext);
				collector.Add(item);
			}
			
			return true;
		}

		public DirectiveAttributeNameItemsProvider([NotNull] T4DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}