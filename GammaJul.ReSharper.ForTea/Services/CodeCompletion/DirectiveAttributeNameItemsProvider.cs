using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	[Language(typeof(T4Language))]
	public class DirectiveAttributeNameItemsProvider : ItemsProviderOfSpecificContext<T4CodeCompletionContext> {

		private readonly DirectiveInfoManager _directiveInfoManager;

		protected override LookupFocusBehaviour GetLookupFocusBehaviour(T4CodeCompletionContext context) {
			return LookupFocusBehaviour.SoftWhenEmpty;
		}

		protected override bool IsAvailable(T4CodeCompletionContext context) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			if (node == null)
				return false;

			var directive = node.Parent as IT4DirectiveAttribute;
			if (directive != null)
				return node.GetTokenType() == T4TokenNodeTypes.Name;

			return node.GetTokenType() == T4TokenNodeTypes.Space && node.Parent is IT4Directive;
		}

		protected override bool AddLookupItems(T4CodeCompletionContext context, GroupedItemsCollector collector) {
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
				.ToHashSet(s => s, StringComparer.OrdinalIgnoreCase);

			foreach (string directiveName in directiveInfo.SupportedAttributes.Select(attr => attr.Name)) {
				if (existingNames.Contains(directiveName))
					continue;
				
				var item = new KeywordLookupItem(directiveName);
				item.InitializeRanges(ranges, context.BasicContext);
				collector.AddAtDefaultPlace(item);
			}
			
			return true;
		}

		public DirectiveAttributeNameItemsProvider([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}