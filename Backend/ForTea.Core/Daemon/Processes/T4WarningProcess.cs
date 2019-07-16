using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Host.Features.Notifications;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	public class T4WarningProcess : T4DaemonStageProcessBase
	{
		private T4DirectiveInfoManager Manager { get; }

		public T4WarningProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file, daemonProcess) => Manager = manager;

		public override void ProcessAfterInterior(ITreeNode element)
		{
			AnalyzeEmptyBlock(element);
			AnalyzeEscapedKeyword(element);
			AnalyzeOutputDirective(element);
		}

		private void AnalyzeOutputDirective([NotNull] ITreeNode element)
		{
//			var solution = element.GetSolution();
//			var host = solution.GetComponent<NotificationPanelHost>();
//			host.AddNotificationPanel(solution.GetLifetime(), null, new NotificationPanel());
		}

		private void AnalyzeEscapedKeyword([NotNull] ITreeNode element)
		{
			if (!(element is T4Token token)) return;
			if (token.NodeType != T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE) return;
			if (!(token.Parent is IT4DirectiveAttribute attribute)) return;
			if (!(attribute.Parent is IT4Directive directive)) return;
			if (!directive.IsSpecificDirective(Manager.Parameter)) return;
			if (attribute.GetName() != Manager.Parameter.TypeAttribute.Name) return;
			if (!CSharpLexer.IsKeyword(token.GetText())) return;
			AddHighlighting(token.GetDocumentRange(), new EscapedKeywordHighlighting(token));
		}

		private void AnalyzeEmptyBlock([NotNull] ITreeNode element)
		{
			if (element is T4FeatureBlock) return;
			if (!(element is T4CodeBlock block)) return;
			if (!block.GetCodeText().IsNullOrWhitespace()) return;
			AddHighlighting(element.GetDocumentRange(), new EmptyBlockHighlighting(block));
		}
	}
}
