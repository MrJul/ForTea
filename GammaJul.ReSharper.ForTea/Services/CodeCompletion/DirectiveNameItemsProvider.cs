using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	[Language(typeof(T4Language))]
	public class DirectiveNameItemsProvider : ItemsProviderOfSpecificContext<T4CodeCompletionContext> {

		private readonly DirectiveInfoManager _directiveInfoManager;

		protected override LookupFocusBehaviour GetLookupFocusBehaviour(T4CodeCompletionContext context) {
			return LookupFocusBehaviour.SoftWhenEmpty;
		}

		protected override bool IsAvailable(T4CodeCompletionContext context) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			if (node == null)
				return false;

			var directive = node.Parent as IT4Directive;
			if (directive == null)
				return false;

			TokenNodeType tokenType = node.GetTokenType();
			IT4Token nameToken = directive.GetNameToken();
			return tokenType == T4TokenNodeTypes.Name
				? nameToken == node
			    : nameToken == null && node.SelfAndLeftSiblings().All(IsWhitespaceOrDirectiveStart);
		}

		private static bool IsWhitespaceOrDirectiveStart(ITreeNode node) {
			TokenNodeType tokenType = node.GetTokenType();
			return tokenType == null
				|| (tokenType.IsWhitespace || tokenType == T4TokenNodeTypes.DirectiveStart);
		}

		protected override bool AddLookupItems(T4CodeCompletionContext context, GroupedItemsCollector collector) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			Assertion.AssertNotNull(node, "node == null");
			var ranges = context.BasicContext.GetRanges(node);
			collector.AddRanges(ranges);

			foreach (string directiveName in _directiveInfoManager.AllDirectives.Select(di => di.Name)) {
				var item = new KeywordLookupItem(directiveName);
				item.InitializeRanges(ranges, context.BasicContext);
				collector.AddAtDefaultPlace(item);
			}
			
			return true;
		}

		public DirectiveNameItemsProvider([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}