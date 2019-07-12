using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public abstract class T4CSharpIntermediateConverterBase
	{
		[NotNull] internal const string TransformTextMethodName = "TransformText";

		[NotNull]
		private T4CSharpCodeGenerationIntermediateResult IntermediateResult { get; }

		[NotNull]
		protected T4CSharpCodeGenerationResult Result { get; }

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpIntermediateConverterBase(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		)
		{
			IntermediateResult = intermediateResult;
			File = file;
			Result = new T4CSharpCodeGenerationResult(File);
		}

		[NotNull]
		public T4CSharpCodeGenerationResult Convert()
		{
			string ns = GetNamespace();
			bool hasNamespace = !string.IsNullOrEmpty(ns);
			if (hasNamespace)
			{
				Result.AppendLine($"namespace {ns}");
				Result.AppendLine("{");
				PushIndent();
				AppendNamespaceContents();
				PopIndent();
				Result.AppendLine("}");
			}
			else
			{
				AppendNamespaceContents();
			}

			return Result;
		}

		private void AppendNamespaceContents()
		{
			AppendImports();
			AppendClasses();
		}

		protected virtual void AppendClasses()
		{
			AppendClass();
			AppendBaseClass();
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

		private void AppendImports()
		{
			AppendIndent();
			Result.AppendLine("using System;");
			Result.Append(IntermediateResult.CollectedImports); // TODO: Indent these, too
		}

		protected void AppendClass()
		{
			AppendSyntheticAttribute();
			AppendIndent();
			Result.Append($"public class {GeneratedClassName} : ");
			AppendBaseClassName();
			Result.AppendLine();
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			if (IntermediateResult.HasHost)
			{
				AppendIndent();
				Result.AppendLine(
					"public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
			}

			AppendTransformMethod();
			Result.Append(IntermediateResult.CollectedFeatures);
			AppendParameterDeclarations(IntermediateResult.ParameterDescriptions);
			AppendTemplateInitialization(IntermediateResult.ParameterDescriptions);
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendParameterDeclarations(
			[NotNull, ItemNotNull] IEnumerable<T4ParameterDescription> descriptions
		)
		{
			foreach (var description in descriptions)
			{
				AppendFieldDeclaration(description);
				AppendParameterDeclaration(description);
			}
		}

		private void AppendFieldDeclaration([NotNull] T4ParameterDescription description)
		{
			AppendSyntheticAttribute();
			AppendIndent();
			Result.Append("private global::");
			Result.Append(description.TypeString);
			Result.Append(" ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		private void AppendTemplateInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			if (descriptions.IsEmpty()) return;
			AppendSyntheticAttribute();
			AppendIndent();
			Result.AppendLine("public void Initialize()");
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			AppendParameterInitialization(descriptions);
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendTransformMethod()
		{
			AppendSyntheticAttribute();
			AppendIndent();
			Result.Append("public ");
			Result.Append(IntermediateResult.HasBaseClass ? "override" : "virtual");
			Result.AppendLine($" string {TransformTextMethodName}()");
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			AppendTransformationPrefix();
			Result.Append(IntermediateResult.CollectedTransformation);
			Result.AppendLine();
			AppendIndent();
			Result.AppendLine("return GenerationEnvironment.ToString();");
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendBaseClassName()
		{
			if (IntermediateResult.HasBaseClass) Result.Append(IntermediateResult.CollectedBaseClass);
			else Result.Append(GeneratedBaseClassName);
		}

		protected void AppendBaseClass()
		{
			if (IntermediateResult.HasBaseClass) return;
			var provider = new T4TemplateResourceProvider(ResourceName, this);
			Result.AppendLine(provider.ProcessResource(GeneratedBaseClassName));
		}

		protected abstract void AppendParameterDeclaration([NotNull] T4ParameterDescription description);

		[NotNull]
		protected abstract string ResourceName { get; }

		[NotNull]
		protected abstract string GeneratedClassName { get; }

		[NotNull]
		protected abstract string GeneratedBaseClassName { get; }

		protected abstract void AppendSyntheticAttribute();

		protected abstract void AppendParameterInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions);

		protected virtual void AppendTransformationPrefix()
		{
		}

		#region Indentation
		private int CurrentIndent { get; set; }
		protected void PushIndent() => CurrentIndent += 1;
		protected void PopIndent() => CurrentIndent -= 1;
		protected void AppendIndent() => AppendIndent(CurrentIndent);
		protected abstract void AppendIndent(int size);
		#endregion Indentation
	}
}
