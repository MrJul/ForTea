using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Impl;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;

namespace GammaJul.ForTea.Core.Services.CodeCompletion {

	[IntellisensePart]
	public class T4CodeCompletionContextProvider : CodeCompletionContextProviderBase {

		public override bool IsApplicable(CodeCompletionContext context)
			=> context.File is IT4File;

		public override ISpecificCodeCompletionContext GetCompletionContext(CodeCompletionContext context)
			=> new T4CodeCompletionContext(context);

	}

}