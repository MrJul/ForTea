using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Helpers
{
	public sealed class T4CSharpCodeGenerationIntermediateResultsConverter :
		T4CSharpCodeGenerationIntermediateResultsConverterBase
	{
		public T4CSharpCodeGenerationIntermediateResultsConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
			GeneratedClassName = File.GetSourceFile()?.Name.WithoutExtension() ?? "GeneratedTransformation";
			GeneratedBaseClassName = GeneratedClassName + "Base";
		}

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseFull.cs";

		protected override string GeneratedClassName { get; }
		protected override string GeneratedBaseClassName { get; }

		protected override void AppendSyntheticAttribute(StringBuilder builder)
		{
			// Synthetic attribute is only used for avoiding completion.
			// It is not valid during compilation,
			// so it should not be inserted in code
		}

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			StringBuilder builder
		)
		{
			builder.AppendLine("            if (Errors.HasErrors) return;");
			foreach (var description in descriptions)
			{
				builder.Append("            if (Session.ContainsKey(nameof(");
				builder.Append(description.FieldNameString);
				builder.AppendLine(")))");
				builder.AppendLine("            {");
				builder.Append("                ");
				builder.Append(description.FieldNameString);
				builder.Append(" = (");
				builder.Append(description.TypeString);
				builder.Append(") Session[nameof(");
				builder.Append(description.FieldNameString);
				builder.AppendLine(")];");
				builder.AppendLine("            }");
				builder.AppendLine("            else");
				builder.AppendLine("            {");
				builder.Append(
					"                object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(nameof(");
				builder.Append(description.FieldNameString);
				builder.AppendLine("));");
				builder.AppendLine("                if (data != null)");
				builder.AppendLine("                {");
				builder.Append("                    ");
				builder.Append(description.FieldNameString);
				builder.Append(" = (");
				builder.Append(description.TypeString);
				builder.AppendLine(") data;");
				builder.AppendLine("                }");
				builder.AppendLine("            }");
			}
		}
	}
}
