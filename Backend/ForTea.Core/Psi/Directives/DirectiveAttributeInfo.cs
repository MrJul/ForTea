using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ForTea.Core.Psi.Directives {

	public class DirectiveAttributeInfo {

		private readonly DirectiveAttributeOptions _options;

		[NotNull]
		public string Name { get; }

		public bool IsRequired
			=> (_options & DirectiveAttributeOptions.Required) == DirectiveAttributeOptions.Required;

		public bool IsDisplayedInCodeStructure
			=> (_options & DirectiveAttributeOptions.DisplayInCodeStructure) == DirectiveAttributeOptions.DisplayInCodeStructure;

		public virtual bool IsValid(string value)
			=> true;

		[NotNull]
		public virtual ImmutableArray<string> IntelliSenseValues
			=> ImmutableArray<string>.Empty;

		[NotNull]
		public IT4DirectiveAttribute CreateDirectiveAttribute([CanBeNull] string value)
			=> T4ElementFactory.CreateDirectiveAttribute(Name, value);

		public DirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options) {
			Name = name;
			_options = options;
		}

	}

}