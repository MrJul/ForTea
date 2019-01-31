using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Parsing {
	
	public static class T4TokenNodeTypes {
		[NotNull] public static readonly T4TokenNodeType Text = new T4TokenNodeType("Text", 1000, null, T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType StatementStart = new T4TokenNodeType("StatementStart", 1001, "<#", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType FeatureStart = new T4TokenNodeType("FeatureStart", 1002, "<#+", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType ExpressionStart = new T4TokenNodeType("ExpressionStart", 1003, "<#=", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType DirectiveStart = new T4TokenNodeType("DirectiveStart", 1004, "<#@", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType BlockEnd = new T4TokenNodeType("BlockEnd", 1005, "#>", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType Space = new T4TokenNodeType("Space", 1006, " ", T4TokenNodeFlag.Whitespace);
		[NotNull] public static readonly T4TokenNodeType NewLine = new T4TokenNodeType("NewLine", 1007, "\r\n", T4TokenNodeFlag.Whitespace);
		[NotNull] public static readonly T4TokenNodeType Quote = new T4TokenNodeType("Quote", 1008, "\"", T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType Equal = new T4TokenNodeType("Equal", 1009, "=", T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType Name = new T4TokenNodeType("Name", 1010, null, T4TokenNodeFlag.Identifier);
		[NotNull] public static readonly T4TokenNodeType Value = new T4TokenNodeType("Value", 1011, null, T4TokenNodeFlag.StringLiteral);
		[NotNull] public static readonly T4TokenNodeType Code = new T4TokenNodeType("Code", 1012, null, T4TokenNodeFlag.None);
	}

}