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
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class ParameterDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _typeAttribute;
		private readonly DirectiveAttributeInfo _nameAttribute;
		private readonly ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		[NotNull]
		public DirectiveAttributeInfo TypeAttribute {
			get { return _typeAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo NameAttribute {
			get { return _nameAttribute; }
		}

		public override ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		public ParameterDirectiveInfo()
			: base("parameter") {

			_typeAttribute = new DirectiveAttributeInfo("type", DirectiveAttributeOptions.Required);
			_nameAttribute = new DirectiveAttributeInfo("name", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);

			_supportedAttributes = Array.AsReadOnly(new[] {
				_typeAttribute,
				_nameAttribute
			});

		}

	}

}