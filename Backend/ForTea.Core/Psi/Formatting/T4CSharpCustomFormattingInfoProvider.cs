using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[ShellComponent]
	public class T4CSharpCustomFormattingInfoProvider : DummyCSharpCustomFormattingInfoProvider
	{
		public override FmtSettings<CSharpFormatSettingsKey> AdjustFormattingSettings(
			FmtSettings<CSharpFormatSettingsKey> settings,
			ISettingsOptimization settingsOptimization
		)
		{
			var cSharpFormatSettings = settings.Settings.Clone();
			int OMG = settings.Settings.INDENT_SIZE; // TODO: Figure out why this is 2 instead of 4!
			cSharpFormatSettings.INDENT_SIZE = 4; // TODO: remove!
			cSharpFormatSettings.OLD_ENGINE = true;
			return settings.ChangeMainSettings(cSharpFormatSettings, true);
		}

		public override SpaceType GetBlockSpaceType(CSharpFmtStageContext ctx, CSharpCodeFormattingContext context)
		{
			var leftChild = ctx.LeftChild;
			var rightChild = ctx.RightChild;

			if (leftChild is ICommentNode &&
			    leftChild.GetText() == T4CSharpCodeBehindGenerationInfoCollector.CodeCommentStart &&
			    !leftChild.HasLineFeedsTo(rightChild, context.CodeFormatter))
			{
				if (ctx.Parent is IClassBody)
				{
					// This doesn't works, because GetBlockSpaceType is called only when right child is comment or inside block.
					// Here we're inside class, not block. Should be fixed in major version.
					/*var statementEnd = leftChild.RightSiblings().FirstOrDefault(n => n is ICommentNode && n.GetText() == AspCSharpCodeBehindGenerator.TRAILING_COMMENT);
					if (statementEnd == null)
					  return SpaceType.Horizontal;
		  
					if (leftChild.HasLineFeedsTo(statementEnd))
					  return SpaceType.Vertical;*/
					return SpaceType.Vertical;
				}

				return SpaceType.Horizontal;
			}

			if (rightChild is ICommentNode &&
			    rightChild.GetText() == T4CSharpCodeBehindGenerationInfoCollector.CodeCommentEnd)
			{
				if (ctx.Parent is IClassBody)
				{
					// This doesn't works, because line break is always inserted after start comment
					/*var start = context.RightChild.LeftSiblings()
					  .FirstOrDefault(c => c is ICommentNode && c.GetText() == AspCSharpCodeBehindGenerator.LEADING_COMMENT);
		  
					if (start == null)
					  return SpaceType.Horizontal;
		  
					return start.HasLineFeedsTo(context.RightChild) ? SpaceType.Vertical : SpaceType.Horizontal;*/
					return SpaceType.Vertical;
				}

				if (leftChild.HasLineFeedsTo(rightChild, context.CodeFormatter))
					return SpaceType.Vertical;
				else
					return SpaceType.Horizontal;
			}

			return SpaceType.Default;
		}

		public override SpaceType GetInvocationSpaces(CSharpFmtStageContext context)
		{
			if (!(context.Parent is IInvocationExpression invocationExpression)) return SpaceType.Default;
			var leftPar = invocationExpression.LPar;
			if (leftPar?.GetDocumentRange().IsValid() == false && context.LeftChild == leftPar)
				return SpaceType.Horizontal;

			var rightPar = invocationExpression.RPar;
			if (rightPar?.GetDocumentRange().IsValid() == false && context.RightChild == rightPar)
				return SpaceType.Horizontal;

			return SpaceType.Default;
		}
	}
}
