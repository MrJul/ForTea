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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class EncodingDirectiveAttributeInfo : DirectiveAttributeInfo {
		private readonly Lazy<JetHashSet<string>> _encodings;
		private ReadOnlyCollection<string> _intellisenseValues;

		[NotNull]
		private static JetHashSet<string> CreateEncodings() {
			return new JetHashSet<string>(Encoding.GetEncodings().Select(info => info.Name), StringComparer.OrdinalIgnoreCase);
		}

		public override bool IsValid(string value) {
			return _encodings.Value.Contains(value);
		}

		public override IEnumerable<string> IntelliSenseValues {
			get { return _intellisenseValues ?? (_intellisenseValues = Array.AsReadOnly(_encodings.Value.ToArray())); }
		}

		public EncodingDirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options)
			: base(name, options) {
			_encodings = Lazy.Of(CreateEncodings, true);
		}

	}

}