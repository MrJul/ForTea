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
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	[Language(typeof(T4Language))]
	public class DirectiveNameItemsProvider : ItemsProviderOfSpecificContext<T4CodeCompletionContext> {

		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;

		protected override LookupFocusBehaviour GetLookupFocusBehaviour(T4CodeCompletionContext context)
			=> LookupFocusBehaviour.SoftWhenEmpty;

		protected override bool IsAvailable(T4CodeCompletionContext context) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			if (!(node?.Parent is IT4Directive directive))
				return false;

			TokenNodeType tokenType = node.GetTokenType();
			IT4Token nameToken = directive.GetNameToken();
			return tokenType == T4TokenNodeTypes.TOKEN
				? nameToken == node
				: nameToken == null && node.SelfAndLeftSiblings().All(IsWhitespaceOrDirectiveStart);
		}

		private static bool IsWhitespaceOrDirectiveStart(ITreeNode node) {
			TokenNodeType tokenType = node.GetTokenType();
			return tokenType == null
				|| (tokenType.IsWhitespace || tokenType == T4TokenNodeTypes.DIRECTIVE_START);
		}

		protected override bool AddLookupItems(T4CodeCompletionContext context, IItemsCollector collector) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			Assertion.AssertNotNull(node, "node == null");
			var ranges = context.BasicContext.GetRanges(node);
			collector.AddRanges(ranges);

			foreach (string directiveName in _directiveInfoManager.AllDirectives.Select(di => di.Name)) {
				var item = new TextLookupItem(directiveName);
				item.InitializeRanges(ranges, context.BasicContext);
				collector.Add(item);
			}
			
			return true;
		}

		public DirectiveNameItemsProvider([NotNull] T4DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}