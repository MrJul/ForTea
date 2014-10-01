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


using JetBrains.Annotations;
using JetBrains.Application;
using Microsoft.VisualStudio.TextTemplating;

namespace GammaJul.ReSharper.ForTea {

	[ShellComponent]
	public sealed class TextTemplatingHostWrapper {

		private readonly Optional<ITextTemplatingEngineHost> _host;

		[NotNull]
		public Optional<ITextTemplatingEngineHost> Host {
			get { return _host; }
		}

		public TextTemplatingHostWrapper([NotNull] ITextTemplatingEngineHost host) {
			// Optional is introduced in ReSharper 8.1. For older versions (7.1 and 8.0), we're faking it.
			// It's becomes non-optional for these versions and will fail in tests, but we don't really have a choice here.
			_host = new Optional<ITextTemplatingEngineHost>(host);
		}

	}

}