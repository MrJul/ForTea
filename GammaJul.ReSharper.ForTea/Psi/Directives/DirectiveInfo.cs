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
using System.Linq;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

	/// <summary>
	/// Contains information about supported T4 directives.
	/// </summary>
	public abstract class DirectiveInfo {

		private readonly string _name;

		[NotNull]
		public string Name {
			get { return _name; }
		}

		public abstract System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes { get; }

		[CanBeNull]
		public DirectiveAttributeInfo GetAttributeByName([CanBeNull] string attributeName) {
			if (String.IsNullOrEmpty(attributeName))
				return null;
			return SupportedAttributes.FirstOrDefault(di => di.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
		}

		[NotNull]
		public IT4Directive CreateDirective([CanBeNull] params Pair<string, string>[] attributes) {
			return T4ElementFactory.Instance.CreateDirective(Name, attributes);
		}

		protected DirectiveInfo([NotNull] string name) {
			_name = name;
		}

	}

}