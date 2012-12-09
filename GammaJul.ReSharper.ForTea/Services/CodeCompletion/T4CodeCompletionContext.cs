using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	public class T4CodeCompletionContext : SpecificCodeCompletionContext {

		public override string ContextId {
			get { return "T4CodeCompletionContext"; }
		}
		
		public T4CodeCompletionContext(CodeCompletionContext context)
			: base(context) {
		}

	}

}