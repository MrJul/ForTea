using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using JetBrains.TextControl.Markup;

[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.Comment, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.HtmlAttributeName, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.HtmlAttributeValue, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.HtmlElementName, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.HtmlOperator, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.HtmlServerSideScript, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.Identifier, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.Keyword, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.Number, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.Operator, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.RazorCode, EffectType = EffectType.HIGHLIGHT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.String, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.StringVerbatim, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.UserType, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.UserTypeDelegate, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.UserTypeEnum, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.UserTypeInterface, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.UserTypeTypeParameter, EffectType = EffectType.TEXT)]
[assembly: RegisterHighlighter(VsPredefinedHighlighterIds.UserTypeValueType, EffectType = EffectType.TEXT)]

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {
	
	public static class VsPredefinedHighlighterIds {
		public const string Comment = "Comment";
		public const string HtmlAttributeName = "HTML Attribute Name";
		public const string HtmlAttributeValue = "HTML Attribute Value";
		public const string HtmlElementName = "HTML Element Name";
		public const string HtmlOperator = "HTML Operator";
		public const string HtmlServerSideScript = "HTML Server-Side Script";
		public const string Identifier = "Identifier";
		public const string Keyword = "Keyword";
		public const string Number = "Number";
		public const string Operator = "Operator";
		public const string RazorCode = "Razor Code";
		public const string String = "String";
		public const string StringVerbatim = "String(C# @ Verbatim)";
		public const string UserType = "User Types";
		public const string UserTypeDelegate = "User Types(Delegates)";
		public const string UserTypeEnum = "User Types(Enums)";
		public const string UserTypeInterface = "User Types(Interfaces)";
		public const string UserTypeTypeParameter = "User Types(Type parameters)";
		public const string UserTypeValueType = "User Types(Value types)";
	}

}