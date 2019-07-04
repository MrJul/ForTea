using System;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators
{
	public abstract class T4CSharpCodeGeneratorBase
	{
		private const string DefaultErrorMessage = "ErrorGeneratingOutput";

		[NotNull]
		protected IT4File File { get; }

		/// <summary>Initializes a new instance of the <see cref="T4CSharpCodeGeneratorBase"/> class.</summary>
		/// <param name="file">The associated T4 file whose C# code behind will be generated.</param>
		protected T4CSharpCodeGeneratorBase([NotNull] IT4File file) =>
			File = file ?? throw new ArgumentNullException(nameof(file));

		[NotNull]
		public T4CSharpCodeGenerationResult Generate()
		{
			try
			{
				return CreateConverter(Collector.Collect()).Convert();
			}
			catch (T4OutputGenerationException)
			{
				var result = new T4CSharpCodeGenerationResult(File);
				result.Append(DefaultErrorMessage);
				return result;
			}
		}

		[NotNull]
		protected abstract T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		[NotNull]
		protected abstract T4CSharpIntermediateConverterBase CreateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult);
	}
}
