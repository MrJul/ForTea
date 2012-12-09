using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Impl;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;

namespace GammaJul.ReSharper.ForTea.Services.CodeCompletion {

	[IntellisensePart]
	public class T4CodeCompletionContextProvider : CodeCompletionContextProviderBase {

		public override bool IsApplicable(CodeCompletionContext context) {
			return context.File is IT4File;
		}

		public override ISpecificCodeCompletionContext GetCompletionContext(CodeCompletionContext context) {
			return new T4CodeCompletionContext(context);
		}

	}

}