using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	public class T4CodeCompletionContext : SpecificCodeCompletionContext {

		public override string ContextId
			=> "T4CodeCompletionContext";

		public T4CodeCompletionContext([NotNull] CodeCompletionContext context)
			: base(context) {
		}

	}

}