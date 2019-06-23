using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Helpers
{
	public abstract class T4CSharpCodeGenerationIntermediateResultsConverterBase
	{
		[NotNull] internal const string TransformTextMethodName = "TransformText";

		[NotNull]
		private T4CSharpCodeGenerationIntermediateResult Result { get; }

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpCodeGenerationIntermediateResultsConverterBase(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		)
		{
			Result = result;
			File = file;
		}

		[NotNull]
		public T4CSharpCodeGenerationResult Convert()
		{
			var result = new T4CSharpCodeGenerationResult(File);
			string ns = GetNamespace();
			bool hasNamespace = !string.IsNullOrEmpty(ns);
			if (hasNamespace)
			{
				result.Builder.AppendLine($"namespace {ns}");
				result.Builder.AppendLine("{");
				AppendNamespaceContents(result);
				result.Builder.AppendLine("}");
			}
			else
			{
				AppendNamespaceContents(result);
			}

			return result;
		}

		[CanBeNull]
		private string GetNamespace()
		{
			var sourceFile = File.GetSourceFile();
			var projectFile = sourceFile?.ToProjectFile();

			string ns = projectFile.GetCustomToolNamespace();
			string ns2 = sourceFile?.Properties.GetDefaultNamespace();
			return T4CSharpCodeGenerationUtils.ChooseBetterNamespace(ns, ns2);
		}

		private void AppendNamespaceContents([NotNull] T4CSharpCodeGenerationResult result)
		{
			result.Builder.AppendLine("using System;");
			result.Append(Result.CollectedImports);
			AppendClass(result);
			AppendBaseClass(result.Builder);
		}

		private void AppendClass([NotNull] T4CSharpCodeGenerationResult result)
		{
			var builder = result.Builder;
			AppendSyntheticAttribute(builder);
			builder.Append($"    public class {GeneratedClassName} : ");
			AppendBaseClassName(result);
			builder.AppendLine();
			builder.AppendLine("    {");
			if (Result.HasHost)
				builder.AppendLine(
					"        public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
			AppendTransformMethod(result);
			result.Append(Result.CollectedFeatures);
			AppendParameterDeclarations(result, Result.ParameterDescriptions);
			AppendTemplateInitialization(result, Result.ParameterDescriptions);
			builder.AppendLine("    }");
		}

		private void AppendParameterDeclarations(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull, ItemNotNull] IEnumerable<T4ParameterDescription> descriptions
		)
		{
			foreach (var description in descriptions)
			{
				AppendFieldDeclaration(result.Builder, description);
				AppendParameterDeclaration(result, description);
			}
		}

		private void AppendFieldDeclaration(
			[NotNull] StringBuilder builder,
			[NotNull] T4ParameterDescription description
		)
		{
			AppendSyntheticAttribute(builder);
			builder.Append("        private global::");
			builder.Append(description.TypeString);
			builder.Append(' ');
			builder.Append(description.FieldNameString);
			builder.AppendLine(";");
		}

		private void AppendParameterDeclaration(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] T4ParameterDescription description
		)
		{
			var builder = result.Builder;
			builder.Append("        private ");
			var type = description.TypeToken;
			if (CSharpLexer.IsKeyword(type.GetText())) builder.Append('@');
			result.AppendMapped(type);
			builder.Append(' ');
			var name = description.NameToken;
			if (CSharpLexer.IsKeyword(name.GetText())) builder.Append('@');
			result.AppendMapped(name);
			builder.Append(" => ");
			builder.Append(description.FieldNameString);
			builder.AppendLine(";");
		}

		private void AppendTemplateInitialization(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			if (descriptions.IsEmpty()) return;
			var builder = result.Builder;
			AppendSyntheticAttribute(builder);
			builder.AppendLine("        public void Initialize()");
			builder.AppendLine("        {");
			AppendParameterInitialization(descriptions, builder);
			builder.AppendLine("        }");
		}

		private void AppendTransformMethod([NotNull] T4CSharpCodeGenerationResult result)
		{
			var builder = result.Builder;
			AppendSyntheticAttribute(builder);
			builder.Append("        public ");
			builder.Append(Result.HasBaseClass ? "override" : "virtual");
			builder.AppendLine($" string {TransformTextMethodName}()");
			builder.AppendLine("        {");
			result.Append(Result.CollectedTransformation);
			builder.AppendLine();
			builder.AppendLine("            return GenerationEnvironment.ToString();");
			builder.AppendLine("        }");
		}

		private void AppendBaseClassName([NotNull] T4CSharpCodeGenerationResult result)
		{
			if (Result.HasBaseClass) result.Append(Result.CollectedBaseClass);
			else result.Builder.Append(GeneratedBaseClassName);
		}

		private void AppendBaseClass([NotNull] StringBuilder builder)
		{
			if (Result.HasBaseClass) return;
			var provider = new T4TemplateBaseProvider(ResourceName);
			builder.AppendLine(provider.CreateTemplateBase(GeneratedBaseClassName));
		}

		[NotNull]
		protected abstract string ResourceName { get; }

		[NotNull]
		protected abstract string GeneratedClassName { get; }

		[NotNull]
		protected abstract string GeneratedBaseClassName { get; }

		protected abstract void AppendSyntheticAttribute([NotNull] StringBuilder builder);

		protected abstract void AppendParameterInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions,
			[NotNull] StringBuilder builder);
	}
}
