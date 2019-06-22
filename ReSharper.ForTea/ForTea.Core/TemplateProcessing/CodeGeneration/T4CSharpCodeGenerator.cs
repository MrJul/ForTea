using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>
	/// This class preprocesses T4 file
	/// to produce C# file that can be compiled and run correctly.
	/// </summary>
	internal sealed class T4CSharpCodeGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file)
		{
			GeneratedClassName = File.GetSourceFile()?.Name.WithExtension("хуй") ?? "хуй.хуй";
			GeneratedBaseClassName = GeneratedClassName + "Base";
			Collector = new T4CSharpCodeGenerationInfoCollector(file, manager);
		}

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseFull.cs";

		protected override void AppendSyntheticAttribute(StringBuilder builder)
		{
		}

		protected override string GeneratedClassName { get; }
		protected override string GeneratedBaseClassName { get; }
		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			StringBuilder builder
		)
		{
			builder.AppendLine("            if (Errors.HasErrors) return;");
			foreach (var description in descriptions)
			{
				builder.Append("            if (Session.ContainsKey(nameof(");
				builder.Append(description.NameString);
				builder.AppendLine(")))");
				builder.AppendLine("            {");
				builder.Append("                ");
				builder.Append(description.NameString);
				builder.Append(" = (");
				builder.Append(description.TypeString);
				builder.Append(") Session[nameof(");
				builder.Append(description.NameString);
				builder.AppendLine(")];");
				builder.AppendLine("            }");
				builder.AppendLine("            else");
				builder.AppendLine("            {");
				builder.Append(
					"                object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(nameof(");
				builder.Append(description.NameString);
				builder.AppendLine("));");
				builder.AppendLine("                if (data != null)");
				builder.AppendLine("                {");
				builder.Append("                    ");
				builder.Append(description.NameString);
				builder.Append(" = (");
				builder.Append(description.TypeString);
				builder.AppendLine(") data;");
				builder.AppendLine("                }");
				builder.AppendLine("            }");
			}
		}
	}
}
