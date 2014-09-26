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

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

    using System;

    using GammaJul.ReSharper.ForTea.Parsing;
    using GammaJul.ReSharper.ForTea.Tree;

    using JetBrains.Annotations;
    using JetBrains.Application.Components;
    using JetBrains.Util;

    using Microsoft.VisualStudio.TextTemplating;

    public class AssemblyDirectiveInfo : DirectiveInfo {

        private readonly Optional<ITextTemplatingEngineHost> ttHost;

        private readonly DirectiveAttributeInfo _nameAttribute;
        private readonly System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> _supportedAttributes;

        [NotNull]
        public DirectiveAttributeInfo NameAttribute {
            get { return _nameAttribute; }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes {
            get { return _supportedAttributes; }
        }

        [NotNull]
        public IT4Directive CreateDirective([NotNull] string assemblyName) {
            return T4ElementFactory.Instance.CreateDirective(Name, Pair.Of(_nameAttribute.Name, assemblyName));
        }

        public AssemblyDirectiveInfo(Optional<ITextTemplatingEngineHost> ttHost)
            : base("assembly") {
            this.ttHost = ttHost;

            _nameAttribute = new DirectiveAttributeInfo("name", DirectiveAttributeOptions.Required | DirectiveAttributeOptions.DisplayInCodeStructure);

            _supportedAttributes = Array.AsReadOnly(new[] {
                _nameAttribute
            });
        }

        public string Resolve(string assemblyNameOrFile) {
            if (this.ttHost.IsNull) {
                return assemblyNameOrFile;
            }

            return this.ttHost.NotNull.ResolveAssemblyReference(assemblyNameOrFile);
        }
    }
}