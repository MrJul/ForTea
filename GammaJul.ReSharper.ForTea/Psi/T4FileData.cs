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
using System.Linq;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	/// <summary>
	/// Contains data for T4 file: which files are included and which assemblies are referenced.
	/// </summary>
	/// <remarks>This class is immutable and thus thread safe.</remarks>
	internal sealed class T4FileData {

		private readonly DirectiveInfoManager _directiveInfoManager;
		private readonly JetHashSet<string> _referencedAssemblies = new JetHashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly JetHashSet<string> _macros = new JetHashSet<string>(StringComparer.OrdinalIgnoreCase);

		private void HandleDirectives([NotNull] IT4DirectiveOwner directiveOwner) {
			foreach (IT4Directive directive in directiveOwner.GetDirectives()) {
				if (directive.IsSpecificDirective(_directiveInfoManager.Assembly))
					HandleAssemblyDirective(directive);
				else if (directive.IsSpecificDirective(_directiveInfoManager.Include))
					HandleIncludeDirective(directive);
			}
		}

		/// <summary>
		/// Handles an assembly directive.
		/// </summary>
		/// <param name="directive">The directive containing a potential assembly reference.</param>
		private void HandleAssemblyDirective([NotNull] IT4Directive directive) {
			string assemblyNameOrFile = directive.GetAttributeValue(_directiveInfoManager.Assembly.NameAttribute.Name);
			if (assemblyNameOrFile == null || (assemblyNameOrFile = assemblyNameOrFile.Trim()).Length == 0)
				return;

			VsBuildMacroHelper.GetMacros(assemblyNameOrFile, _macros);
			_referencedAssemblies.Add(assemblyNameOrFile);
		}

		/// <summary>
		/// Handles an include directive.
		/// </summary>
		/// <param name="directive">The directive containing a potential macro.</param>
		private void HandleIncludeDirective([NotNull] IT4Directive directive) {
			VsBuildMacroHelper.GetMacros(directive.GetAttributeValue(_directiveInfoManager.Include.FileAttribute.Name), _macros);
		}

		/// <summary>
		/// Computes a difference between this data and another one.
		/// </summary>
		/// <param name="oldData">The old data.</param>
		/// <returns>
		/// An instance of <see cref="T4FileDataDiff"/> containing the difference between the two data,
		/// or <c>null</c> if there are no differences.
		/// </returns>
		[CanBeNull]
		internal T4FileDataDiff DiffWith([CanBeNull] T4FileData oldData) {

			if (oldData == null) {
				if (_referencedAssemblies.Count == 0 && _macros.Count == 0)
					return null;
				return new T4FileDataDiff(_referencedAssemblies, EmptyList<string>.InstanceList, _macros);
			}

			string[] addedMacros = _macros.Except(oldData._macros).ToArray();

			JetHashSet<string> addedAssemblies;
			JetHashSet<string> removedAssemblies;
			oldData._referencedAssemblies.Compare(_referencedAssemblies, out addedAssemblies, out removedAssemblies);
			
			if (addedMacros.Length == 0 && addedAssemblies.Count == 0 && removedAssemblies.Count == 0)
				return null;
			return new T4FileDataDiff(addedAssemblies, removedAssemblies, addedMacros);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4FileData"/> class.
		/// </summary>
		/// <param name="t4File">The T4 file that will be scanned for data.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		internal T4FileData([NotNull] IT4File t4File, [NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
			
			HandleDirectives(t4File);
			foreach (IT4Include include in t4File.GetRecursiveIncludes())
				HandleDirectives(include);
		}

	}

}