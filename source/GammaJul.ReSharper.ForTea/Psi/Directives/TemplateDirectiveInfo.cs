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
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	public class TemplateDirectiveInfo : DirectiveInfo {

		private readonly DirectiveAttributeInfo _languageAttribute;
		private readonly DirectiveAttributeInfo _hostSpecificAttribute;
		private readonly DirectiveAttributeInfo _debugAttribute;
		private readonly DirectiveAttributeInfo _inheritsAttribute;
		private readonly DirectiveAttributeInfo _cultureAttribute;
		private readonly DirectiveAttributeInfo _compilerOptionsAttribute;
		private readonly DirectiveAttributeInfo _linePragmasAttribute;
		private readonly DirectiveAttributeInfo _visibilityAttribute;
		private readonly ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

		[NotNull]
		public DirectiveAttributeInfo CompilerOptionsAttribute {
			get { return _compilerOptionsAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo CultureAttribute {
			get { return _cultureAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo DebugAttribute {
			get { return _debugAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo HostSpecificAttribute {
			get { return _hostSpecificAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo InheritsAttribute {
			get { return _inheritsAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo LanguageAttribute {
			get { return _languageAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo LinePragmasAttribute {
			get { return _linePragmasAttribute; }
		}

		[NotNull]
		public DirectiveAttributeInfo VisibilityAttribute {
			get { return _visibilityAttribute; }
		}

		public override ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
			get { return _supportedAttributes; }
		}

		public TemplateDirectiveInfo([NotNull] T4Environment environment)
			: base("template") {

			bool isAtLeastVs2012 = environment.VsVersion2.Major >= VsVersions.Vs2012;

			_languageAttribute = new EnumDirectiveAttributeInfo("language", DirectiveAttributeOptions.None, "C#", "VB");
			_hostSpecificAttribute = isAtLeastVs2012
				? new EnumDirectiveAttributeInfo("hostspecific",DirectiveAttributeOptions.None, "true", "false", "trueFromBase")
				: new BooleanDirectiveAttributeInfo("hostspecific", DirectiveAttributeOptions.None);
			_debugAttribute = new BooleanDirectiveAttributeInfo("debug", DirectiveAttributeOptions.None);
			_inheritsAttribute = new DirectiveAttributeInfo("inherits", DirectiveAttributeOptions.None);
			_cultureAttribute = new CultureDirectiveAttributeInfo("culture", DirectiveAttributeOptions.None);
			_compilerOptionsAttribute = new DirectiveAttributeInfo("compilerOptions", DirectiveAttributeOptions.None);
			_linePragmasAttribute = new BooleanDirectiveAttributeInfo("linePragmas", DirectiveAttributeOptions.None);
			_visibilityAttribute = new EnumDirectiveAttributeInfo("visibility", DirectiveAttributeOptions.None, "public", "internal");

			var attributes = new List<DirectiveAttributeInfo>(8) {
				_languageAttribute,
				_hostSpecificAttribute,
				_debugAttribute,
				_inheritsAttribute,
				_cultureAttribute,
				_compilerOptionsAttribute
			};

			if (isAtLeastVs2012) {
				attributes.Add(_linePragmasAttribute);
				attributes.Add(_visibilityAttribute);
			}

			_supportedAttributes = attributes.AsReadOnly();
		}

	}

}