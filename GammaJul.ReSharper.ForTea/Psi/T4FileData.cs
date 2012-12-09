using System;
using System.Collections.Generic;
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

		private void HandleAssemblyDirectives([NotNull] IT4DirectiveOwner directiveOwner) {
			foreach (IT4Directive directive in directiveOwner.GetDirectives(_directiveInfoManager.Assembly))
				HandleAssemblyDirective(directive);
		}

		/// <summary>
		/// Handles an assembly directive.
		/// </summary>
		/// <param name="directive">The directive containing a potential assembly reference.</param>
		private void HandleAssemblyDirective([NotNull] IT4Directive directive) {
			string assemblyName = directive.GetAttributeValue(_directiveInfoManager.Assembly.NameAttribute.Name);
			if (assemblyName == null || (assemblyName = assemblyName.Trim()).Length == 0)
				return;

			_referencedAssemblies.Add(assemblyName);
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
				if (_referencedAssemblies.Count == 0)
					return null;
				return new T4FileDataDiff(_referencedAssemblies, EmptyList<string>.InstanceList);
			}

			JetHashSet<string> addedAssemblies;
			JetHashSet<string> removedAssemblies;
			oldData._referencedAssemblies.Compare(_referencedAssemblies, out addedAssemblies, out removedAssemblies);
			if (addedAssemblies.Count == 0 && removedAssemblies.Count == 0)
				return null;
			return new T4FileDataDiff(addedAssemblies, removedAssemblies);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4FileData"/> class.
		/// </summary>
		/// <param name="t4File">The T4 file that will be scanned for data.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		internal T4FileData([NotNull] IT4File t4File, DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;

			HandleAssemblyDirectives(t4File);
			foreach (IT4Include include in t4File.GetRecursiveIncludes())
				HandleAssemblyDirectives(include);
		}

	}

}