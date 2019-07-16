using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Parsing {
	
	public static class T4TokenNodeTypes
	{
		[NotNull] public static readonly T4TokenNodeType BAD_TOKEN = new T4TokenNodeType("BAD_TOKEN", 999, null, T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType RAW_TEXT = new T4TokenNodeType("RAW_TEXT", 1000, null, T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType STATEMENT_BLOCK_START = new T4TokenNodeType("STATEMENT_BLOCK_START", 1001, "<#", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType FEATURE_BLOCK_START = new T4TokenNodeType("FEATURE_BLOCK_START", 1002, "<#+", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType EXPRESSION_BLOCK_START = new T4TokenNodeType("EXPRESSION_BLOCK_START", 1003, "<#=", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType DIRECTIVE_START = new T4TokenNodeType("DIRECTIVE_START", 1004, "<#@", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType BLOCK_END = new T4TokenNodeType("BLOCK_END", 1005, "#>", T4TokenNodeFlag.Tag);
		[NotNull] public static readonly T4TokenNodeType WHITE_SPACE = new T4TokenNodeType("WHITE_SPACE", 1006, " ", T4TokenNodeFlag.Whitespace);
		[NotNull] public static readonly T4TokenNodeType NEW_LINE = new T4TokenNodeType("NEW_LINE", 1007, "\r\n", T4TokenNodeFlag.Whitespace);
		[NotNull] public static readonly T4TokenNodeType QUOTE = new T4TokenNodeType("QUOTE", 1008, "\"", T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType EQUAL = new T4TokenNodeType("EQUAL", 1009, "=", T4TokenNodeFlag.None);
		[NotNull] public static readonly T4TokenNodeType TOKEN = new T4TokenNodeType("TOKEN", 1010, null, T4TokenNodeFlag.Identifier);
		[NotNull] public static readonly T4TokenNodeType RAW_ATTRIBUTE_VALUE = new T4TokenNodeType("RAW_ATTRIBUTE_VALUE", 1011, null, T4TokenNodeFlag.StringLiteral);
		[NotNull] public static readonly T4TokenNodeType RAW_CODE = new T4TokenNodeType("RAW_CODE", 1012, null, T4TokenNodeFlag.None);
	}

}
