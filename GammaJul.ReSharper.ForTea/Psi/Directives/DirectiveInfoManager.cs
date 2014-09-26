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
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;

namespace GammaJul.ReSharper.ForTea.Psi.Directives {

    using JetBrains.Application.Components;

    using Microsoft.VisualStudio.TextTemplating;

    [ShellComponent]
    public class DirectiveInfoManager {

        /// <summary>
        /// Gets information about the template directive.
        /// </summary>
        [NotNull]
        public readonly TemplateDirectiveInfo Template;

        /// <summary>
        /// Gets information about the parameter directive.
        /// </summary>
        [NotNull]
        public readonly ParameterDirectiveInfo Parameter;

        /// <summary>
        /// Gets information about the output directive.
        /// </summary>
        [NotNull]
        public readonly OutputDirectiveInfo Output;

        /// <summary>
        /// Gets information about the include directive.
        /// </summary>
        [NotNull]
        public readonly IncludeDirectiveInfo Include;

        /// <summary>
        /// Gets information about the assembly directive.
        /// </summary>
        [NotNull]
        public readonly AssemblyDirectiveInfo Assembly;

        /// <summary>
        /// Gets information about the import directive.
        /// </summary>
        [NotNull]
        public readonly ImportDirectiveInfo Import;

        /// <summary>
        /// Gets a collection of all known directives.
        /// </summary>
        /// <remarks>Order of elements in this collection will be used to order the directives.</remarks>
        [NotNull]
        public readonly ReadOnlyCollection<DirectiveInfo> AllDirectives;

        [CanBeNull]
        public DirectiveInfo GetDirectiveByName([CanBeNull] string directiveName) {
            if (String.IsNullOrEmpty(directiveName))
                return null;
            return AllDirectives.FirstOrDefault(di => di.Name.Equals(directiveName, StringComparison.OrdinalIgnoreCase));
        }

        public DirectiveInfoManager([NotNull] T4Environment environment, Optional<ITextTemplatingEngineHost> ttHost) {
            Template = new TemplateDirectiveInfo(environment);
            Parameter = new ParameterDirectiveInfo();
            Output = new OutputDirectiveInfo();
            Include = new IncludeDirectiveInfo(environment);
            Assembly = new AssemblyDirectiveInfo(ttHost);
            Import = new ImportDirectiveInfo();
            AllDirectives = Array.AsReadOnly(new DirectiveInfo[] {
                Template,
                Parameter,
                Output,
                Include,
                Assembly,
                Import
            });
        }

    }

}