using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
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
	}
}
