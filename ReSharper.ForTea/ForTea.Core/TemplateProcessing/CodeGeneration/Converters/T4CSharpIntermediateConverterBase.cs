using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public abstract class T4CSharpIntermediateConverterBase
	{
		[NotNull] internal const string TransformTextMethodName = "TransformText";

		[NotNull]
		private T4CSharpCodeGenerationIntermediateResult Result { get; }

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpIntermediateConverterBase(
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
				result.AppendLine($"namespace {ns}");
				result.AppendLine("{");
				AppendNamespaceContents(result);
				result.AppendLine("}");
			}
			else
			{
				AppendNamespaceContents(result);
			}

			return result;
		}

		private void AppendNamespaceContents([NotNull] T4CSharpCodeGenerationResult result)
		{
			AppendNamespaceContentPrefix(result);
			AppendImports(result);
			AppendClass(result);
			AppendBaseClass(result);
			AppendNamespaceContentSuffix(result);
		}

		protected virtual void AppendNamespaceContentSuffix([NotNull] T4CSharpCodeGenerationResult result)
		{
		}

		protected virtual void AppendNamespaceContentPrefix([NotNull] T4CSharpCodeGenerationResult result)
		{
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

		private void AppendImports(T4CSharpCodeGenerationResult result)
		{
			result.AppendLine("using System;");
			result.Append(Result.CollectedImports);
		}

		private void AppendClass([NotNull] T4CSharpCodeGenerationResult result)
		{
			AppendSyntheticAttribute(result);
			result.Append($"    public class {GeneratedClassName} : ");
			AppendBaseClassName(result);
			result.AppendLine();
			result.AppendLine("    {");
			if (Result.HasHost)
			{
				result.AppendLine(
					"        public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
			}

			AppendTransformMethod(result);
			result.Append(Result.CollectedFeatures);
			AppendParameterDeclarations(result, Result.ParameterDescriptions);
			AppendTemplateInitialization(result, Result.ParameterDescriptions);
			result.AppendLine("    }");
		}

		private void AppendParameterDeclarations(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull, ItemNotNull] IEnumerable<T4ParameterDescription> descriptions
		)
		{
			foreach (var description in descriptions)
			{
				AppendFieldDeclaration(result, description);
				AppendParameterDeclaration(result, description);
			}
		}

		private void AppendFieldDeclaration(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] T4ParameterDescription description
		)
		{
			AppendSyntheticAttribute(result);
			result.Append("        private global::");
			result.Append(description.TypeString);
			result.Append(" ");
			result.Append(description.FieldNameString);
			result.AppendLine(";");
		}

		private void AppendParameterDeclaration(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] T4ParameterDescription description
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

		private void AppendTemplateInitialization(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			if (descriptions.IsEmpty()) return;
			AppendSyntheticAttribute(result);
			result.AppendLine("        public void Initialize()");
			result.AppendLine("        {");
			AppendParameterInitialization(descriptions, result);
			result.AppendLine("        }");
		}

		private void AppendTransformMethod([NotNull] T4CSharpCodeGenerationResult result)
		{
			AppendSyntheticAttribute(result);
			result.Append("        public ");
			result.Append(Result.HasBaseClass ? "override" : "virtual");
			result.AppendLine($" string {TransformTextMethodName}()");
			result.AppendLine("        {");
			result.Append(Result.CollectedTransformation);
			result.AppendLine();
			result.AppendLine("            return GenerationEnvironment.ToString();");
			result.AppendLine("        }");
		}

		private void AppendBaseClassName([NotNull] T4CSharpCodeGenerationResult result)
		{
			if (Result.HasBaseClass) result.Append(Result.CollectedBaseClass);
			else result.Append(GeneratedBaseClassName);
		}

		private void AppendBaseClass([NotNull] T4CSharpCodeGenerationResult result)
		{
			if (Result.HasBaseClass) return;
			var provider = new T4TemplateResourceProvider(ResourceName, this);
			result.AppendLine(provider.ProcessResource(GeneratedBaseClassName));
		}

		[NotNull]
		protected abstract string ResourceName { get; }

		[NotNull]
		protected abstract string GeneratedClassName { get; }

		[NotNull]
		protected abstract string GeneratedBaseClassName { get; }

		protected abstract void AppendSyntheticAttribute([NotNull] T4CSharpCodeGenerationResult result);

		protected abstract void AppendParameterInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions,
			[NotNull] T4CSharpCodeGenerationResult result);
	}
}
