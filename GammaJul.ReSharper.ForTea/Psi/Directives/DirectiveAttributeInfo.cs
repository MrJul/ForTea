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
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class DirectiveAttributeInfo {
		private readonly string _name;
		private readonly DirectiveAttributeOptions _options;

		[NotNull]
		public string Name {
			get { return _name; }
		}

		public bool IsRequired {
			get { return (_options & DirectiveAttributeOptions.Required) == DirectiveAttributeOptions.Required; }
		}

		public bool IsDisplayedInCodeStructure {
			get { return (_options & DirectiveAttributeOptions.DisplayInCodeStructure) == DirectiveAttributeOptions.DisplayInCodeStructure; }
		}

		public virtual bool IsValid(string value) {
			return true;
		}

		[NotNull]
		public virtual IEnumerable<string> IntelliSenseValues {
			get { return EmptyList<string>.InstanceList; }
		}

		public DirectiveAttributeInfo([NotNull] string name, DirectiveAttributeOptions options) {
			_name = name;
			_options = options;
		}

	}

}