using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpCodeBehindIntermediateConverter : T4CSharpIntermediateConverterBase
	{
		[NotNull] public const string GeneratedClassNameString = "Generated\x200CTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = GeneratedClassNameString + "Base";

		[NotNull, ItemNotNull] private string[] DisabledPropertyInspections =
		{
			"BuiltInTypeReferenceStyle",
			"RedundantNameQualifier"
		};

		public T4CSharpCodeBehindIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		protected override string ResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseStub.cs";

		protected override string GeneratedClassName => GeneratedClassNameString;
		protected override string GeneratedBaseClassName => GeneratedBaseClassNameString;

		protected override void AppendSyntheticAttribute(T4CSharpCodeGenerationResult result) =>
			result.AppendLine($"        [{SyntheticAttribute.Name}]");

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			T4CSharpCodeGenerationResult result)
		{
			// There's no need to initialize parameters in code-behind since this code is never displayed anyway 
		}

		protected override void AppendParameterDeclaration(
			T4CSharpCodeGenerationResult result,
			T4ParameterDescription description
		)
		{
			foreach (string inspection in DisabledPropertyInspections)
			{
				AppendDisabledInspections(result, inspection);
			}

			result.Append("        private global::");
			var type = description.TypeToken;
			if (CSharpLexer.IsKeyword(type.GetText())) result.Append("@");
			result.AppendMapped(type);
			result.Append(" ");
			var name = description.NameToken;
			if (CSharpLexer.IsKeyword(name.GetText())) result.Append("@");
			result.AppendMapped(name);
			result.Append(" => ");
			result.Append(description.FieldNameString);
			result.AppendLine(";");
		}

		private void AppendDisabledInspections(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] string inspection
		)
		{
			result.Append("        // ReSharper disable once ");
			result.AppendLine(inspection);
		}
	}
}
