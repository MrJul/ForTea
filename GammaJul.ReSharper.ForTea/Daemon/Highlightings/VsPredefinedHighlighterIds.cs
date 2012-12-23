#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
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