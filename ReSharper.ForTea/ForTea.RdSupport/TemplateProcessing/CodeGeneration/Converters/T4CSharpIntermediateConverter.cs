using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Converters
{
	public class T4CSharpIntermediateConverter : T4CSharpIntermediateConverterBase
	{
		public T4CSharpIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
			GeneratedClassName = File.GetSourceFile()?.Name.WithoutExtension() ?? "GeneratedTransformation";
			GeneratedBaseClassName = GeneratedClassName + "Base";
		}

		protected sealed override string ResourceName =>
			"JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.TemplateBaseFull.cs";

		protected sealed override string GeneratedClassName { get; }
		protected sealed override string GeneratedBaseClassName { get; }

		protected sealed override void AppendSyntheticAttribute(StringBuilder builder)
		{
			// Synthetic attribute is only used for avoiding completion.
			// It is not valid during compilation,
			// so it should not be inserted in code
		}

		protected sealed override void AppendParameterInitialization(
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
