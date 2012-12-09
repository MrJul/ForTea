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
	public class DirectiveAttributeValueItemsProvider : ItemsProviderOfSpecificContext<T4CodeCompletionContext> {

		private readonly DirectiveInfoManager _directiveInfoManager;

		protected override LookupFocusBehaviour GetLookupFocusBehaviour(T4CodeCompletionContext context) {
			return LookupFocusBehaviour.SoftWhenEmpty;
		}

		protected override bool IsAvailable(T4CodeCompletionContext context) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			if (node == null)
				return false;

			var directive = node.Parent as IT4DirectiveAttribute;
			if (directive == null)
				return false;

			TokenNodeType tokenType = node.GetTokenType();
			if (tokenType == T4TokenNodeTypes.Value)
				return true;

			if (tokenType == T4TokenNodeTypes.Quote) {
				ITreeNode leftSibling = node.GetMeaningfulLeftSib();
				if (leftSibling != null && leftSibling.GetTokenType() == T4TokenNodeTypes.Equal)
					return true;
			}

			return false;
		}

		protected override bool AddLookupItems(T4CodeCompletionContext context, GroupedItemsCollector collector) {
			ITreeNode node = context.BasicContext.File.FindNodeAt(context.BasicContext.SelectedTreeRange);
			Assertion.AssertNotNull(node, "node == null");
			var ranges = context.BasicContext.GetRanges(node);
			collector.AddRanges(ranges);

			var attribute = node.GetContainingNode<IT4DirectiveAttribute>();
			Assertion.AssertNotNull(attribute, "attribute != null");

			var directive = attribute.GetContainingNode<IT4Directive>();
			Assertion.AssertNotNull(directive, "directive != null");

			DirectiveInfo directiveInfo = _directiveInfoManager.GetDirectiveByName(directive.GetName());
			if (directiveInfo == null)
				return false;

			DirectiveAttributeInfo attributeInfo = directiveInfo.GetAttributeByName(attribute.GetName());
			if (attributeInfo == null)
				return false;
			
			foreach (string intellisenseValue in attributeInfo.IntelliSenseValues) {
				var item = new KeywordLookupItem(intellisenseValue);
				item.InitializeRanges(ranges, context.BasicContext);
				collector.AddAtDefaultPlace(item);
			}
			return true;
		}

		public DirectiveAttributeValueItemsProvider([NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}