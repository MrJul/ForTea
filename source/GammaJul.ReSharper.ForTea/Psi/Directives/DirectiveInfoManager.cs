using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataStructures;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	[ShellComponent]
	public class DirectiveInfoManager {

		/// <summary>Gets information about the template directive.</summary>
		[NotNull]
		public readonly TemplateDirectiveInfo Template;

		/// <summary>Gets information about the parameter directive.</summary>
		[NotNull]
		public readonly ParameterDirectiveInfo Parameter;

		/// <summary>Gets information about the output directive.</summary>
		[NotNull]
		public readonly OutputDirectiveInfo Output;

		/// <summary>Gets information about the include directive.</summary>
		[NotNull]
		public readonly IncludeDirectiveInfo Include;

		/// <summary>Gets information about the assembly directive.</summary>
		[NotNull]
		public readonly AssemblyDirectiveInfo Assembly;

		/// <summary>Gets information about the import directive.</summary>
		[NotNull]
		public readonly ImportDirectiveInfo Import;

		/// <summary>Gets a collection of all known directives.</summary>
		/// <remarks>Order of elements in this collection will be used to order the directives.</remarks>
		[NotNull]
		[ItemNotNull]
		public readonly ImmutableArray<DirectiveInfo> AllDirectives;

		[CanBeNull]
		public DirectiveInfo GetDirectiveByName([CanBeNull] string directiveName)
			=> String.IsNullOrEmpty(directiveName)
				? null
				: AllDirectives.FirstOrDefault(di => di.Name.Equals(directiveName, StringComparison.OrdinalIgnoreCase));

		public DirectiveInfoManager([NotNull] T4Environment environment) {
			Template = new TemplateDirectiveInfo(environment);
			Parameter = new ParameterDirectiveInfo();
			Output = new OutputDirectiveInfo();
			Include = new IncludeDirectiveInfo(environment);
			Assembly = new AssemblyDirectiveInfo();
			Import = new ImportDirectiveInfo();
			AllDirectives = ImmutableArray.FromArguments(new DirectiveInfo[] {
				Template,
				Parameter,
				Output,
				Include,
				Assembly,
				Import
			});
		}

	}

}