#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
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