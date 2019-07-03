using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
	[QuickFix]
	public class ReplaceWithClrNameQuickFix : QuickFixBase
	{
		[NotNull]
		private EscapedKeywordHighlighting Highlighting { get; }

		public ReplaceWithClrNameQuickFix([NotNull] EscapedKeywordHighlighting highlighting) =>
			Highlighting = highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var node = Highlighting.AssociatedNode;
			string keyword = node.GetText();
			Assertion.Assert(CSharpLexer.IsKeyword(keyword), "CSharpLexer.IsKeyword(text)");
			
			var qualified = CSharpTypeFactory.GetFullyQualifiedNameByKeyword(keyword).NotNull();
			var newNode = T4ElementFactory.CreateToken(qualified.FullName);
			var file = node.GetContainingFile().NotNull();
			
			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				ModificationUtil.ReplaceChild(node, newNode);
			}

			return null;
		}

		public override string Text => "Replace with CLR name";
		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
	}
}
