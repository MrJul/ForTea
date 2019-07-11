using System.Collections.Generic;
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
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		) : base(intermediateResult, file)
		{
		}

		protected sealed override string ResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseFull.cs";

		protected override string GeneratedClassName =>
			File.GetSourceFile()?.Name.WithoutExtension() ?? DefaultGeneratedClassName;

		protected sealed override string GeneratedBaseClassName => GeneratedClassName + "Base";

		protected sealed override void AppendSyntheticAttribute()
		{
			// Synthetic attribute is only used for avoiding completion.
			// It is not valid during compilation,
			// so it should not be inserted in code
		}

		protected sealed override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			AppendIndent();
			Result.AppendLine("if (Errors.HasErrors) return;");
			foreach (var description in descriptions)
			{
				AppendIndent();
				Result.Append("if (Session.ContainsKey(nameof(");
				Result.Append(description.FieldNameString);
				Result.AppendLine(")))");
				AppendIndent();
				Result.AppendLine("{");
				PushIndent();
				AppendIndent();
				Result.Append(description.FieldNameString);
				Result.Append(" = (");
				Result.Append(description.TypeString);
				Result.Append(") Session[nameof(");
				Result.Append(description.FieldNameString);
				Result.AppendLine(")];");
				PopIndent();
				AppendIndent();
				Result.AppendLine("}");
				AppendIndent();
				Result.AppendLine("else");
				AppendIndent();
				Result.AppendLine("{");
				PushIndent();
				AppendIndent();
				Result.Append(
					"object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(nameof(");
				Result.Append(description.FieldNameString);
				Result.AppendLine("));");
				AppendIndent();
				Result.AppendLine("if (data != null)");
				AppendIndent();
				Result.AppendLine("{");
				PushIndent();
				AppendIndent();
				Result.Append(description.FieldNameString);
				Result.Append(" = (");
				Result.Append(description.TypeString);
				Result.AppendLine(") data;");
				PopIndent();
				AppendIndent();
				Result.AppendLine("}");
				PopIndent();
				AppendIndent();
				Result.AppendLine("}");
			}
		}

		protected override void AppendParameterDeclaration(T4ParameterDescription description)
		{
			AppendIndent();
			Result.Append("private ");
			var type = description.TypeToken;
			if (CSharpLexer.IsKeyword(type.GetText())) Result.Append("@");
			Result.AppendMapped(type);
			Result.Append(" ");
			var name = description.NameToken;
			if (CSharpLexer.IsKeyword(name.GetText())) Result.Append("@");
			Result.AppendMapped(name);
			Result.Append(" => ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		protected override void AppendIndent(int size)
		{
			// TODO: use user indents?
			for (int index = 0; index < 10; index += 1)
			{
				Result.Append("    ");
			}
		}
	}
}
