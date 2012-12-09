using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Parsing {
	
	public static class T4TokenNodeTypes {
		[NotNull] public static readonly T4TokenNodeType Text = new T4TokenNodeType("Text", null, T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType StatementStart = new T4TokenNodeType("StatementStart", "<#", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType FeatureStart = new T4TokenNodeType("FeatureStart", "<#+", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType ExpressionStart = new T4TokenNodeType("ExpressionStart", "<#=", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType DirectiveStart = new T4TokenNodeType("DirectiveStart", "<#@", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType BlockEnd = new T4TokenNodeType("BlockEnd", "#>", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType Space = new T4TokenNodeType("Space", " ", T4TokenNodeFlag.Whitespace);
		[NotNull] public static readonly T4TokenNodeType NewLine = new T4TokenNodeType("NewLine", "\r\n", T4TokenNodeFlag.Whitespace);
		[NotNull] public static readonly T4TokenNodeType Quote = new T4TokenNodeType("Quote", "\"", T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType Equal = new T4TokenNodeType("Equal", "=", T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType Name = new T4TokenNodeType("Name", null, T4TokenNodeFlag.Identifier);
		[NotNull] public static readonly T4TokenNodeType Value = new T4TokenNodeType("Value", null, T4TokenNodeFlag.StringLiteral);
		[NotNull] public static readonly T4TokenNodeType Code = new T4TokenNodeType("Code", null, T4TokenNodeFlag.None);
	}

}