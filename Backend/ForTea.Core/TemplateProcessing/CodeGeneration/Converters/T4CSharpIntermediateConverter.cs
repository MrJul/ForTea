using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public class T4CSharpIntermediateConverter : T4CSharpIntermediateConverterBase
	{
		protected const string DefaultGeneratedClassName = "GeneratedTransformation";

		public T4CSharpIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		protected sealed override string ResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseFull.cs";

		protected override string GeneratedClassName =>
			File.GetSourceFile()?.Name.WithoutExtension() ?? DefaultGeneratedClassName;

		protected sealed override string GeneratedBaseClassName => GeneratedClassName + "Base";

		protected sealed override void AppendSyntheticAttribute(T4CSharpCodeGenerationResult result)
		{
			// Synthetic attribute is only used for avoiding completion.
			// It is not valid during compilation,
			// so it should not be inserted in code
		}

		protected sealed override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			T4CSharpCodeGenerationResult result)
		{
			result.AppendLine("            if (Errors.HasErrors) return;");
			foreach (var description in descriptions)
			{
				result.Append("            if (Session.ContainsKey(nameof(");
				result.Append(description.FieldNameString);
				result.AppendLine(")))");
				result.AppendLine("            {");
				result.Append("                ");
				result.Append(description.FieldNameString);
				result.Append(" = (");
				result.Append(description.TypeString);
				result.Append(") Session[nameof(");
				result.Append(description.FieldNameString);
				result.AppendLine(")];");
				result.AppendLine("            }");
				result.AppendLine("            else");
				result.AppendLine("            {");
				result.Append(
					"                object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(nameof(");
				result.Append(description.FieldNameString);
				result.AppendLine("));");
				result.AppendLine("                if (data != null)");
				result.AppendLine("                {");
				result.Append("                    ");
				result.Append(description.FieldNameString);
				result.Append(" = (");
				result.Append(description.TypeString);
				result.AppendLine(") data;");
				result.AppendLine("                }");
				result.AppendLine("            }");
			}
		}

		protected override void AppendParameterDeclaration(
			T4CSharpCodeGenerationResult result,
			T4ParameterDescription description
		)
		{
			result.Append("        private ");
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
	}
}
