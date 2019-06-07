using System.Collections.Generic;
using GammaJul.ForTea.Core.Common;
using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ForTea.Core.Psi.Directives
{
	public class TemplateDirectiveInfo : DirectiveInfo
	{
		[NotNull]
		public DirectiveAttributeInfo CompilerOptionsAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo CultureAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo DebugAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo HostSpecificAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo InheritsAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo LanguageAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo LinePragmasAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo VisibilityAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public TemplateDirectiveInfo([NotNull] IT4Environment environment) : base("template")
		{
			LanguageAttribute = new EnumDirectiveAttributeInfo("language", DirectiveAttributeOptions.None, "C#", "VB");
			HostSpecificAttribute = BuildHostSpecificAttribute(environment);
			DebugAttribute = new BooleanDirectiveAttributeInfo("debug", DirectiveAttributeOptions.None);
			InheritsAttribute = new DirectiveAttributeInfo("inherits", DirectiveAttributeOptions.None);
			CultureAttribute = new CultureDirectiveAttributeInfo("culture", DirectiveAttributeOptions.None);
			CompilerOptionsAttribute = new DirectiveAttributeInfo("compilerOptions", DirectiveAttributeOptions.None);
			LinePragmasAttribute = new BooleanDirectiveAttributeInfo("linePragmas", DirectiveAttributeOptions.None);
			VisibilityAttribute =
				new EnumDirectiveAttributeInfo("visibility", DirectiveAttributeOptions.None, "public", "internal");

			var attributes = new List<DirectiveAttributeInfo>(8)
			{
				LanguageAttribute,
				HostSpecificAttribute,
				DebugAttribute,
				InheritsAttribute,
				CultureAttribute,
				CompilerOptionsAttribute
			};

			if (environment.ShouldSupportAdvancedAttributes)
			{
				attributes.Add(LinePragmasAttribute);
				attributes.Add(VisibilityAttribute);
			}

			SupportedAttributes = attributes.ToImmutableArray();
		}

		private static EnumDirectiveAttributeInfo BuildHostSpecificAttribute(IT4Environment environment)
		{
			if (!environment.ShouldSupportAdvancedAttributes)
				return new BooleanDirectiveAttributeInfo("hostspecific", DirectiveAttributeOptions.None);

			return new EnumDirectiveAttributeInfo(
				"hostspecific",
				DirectiveAttributeOptions.None,
				"true",
				"false",
				"trueFromBase"
			);
		}
	}
}
