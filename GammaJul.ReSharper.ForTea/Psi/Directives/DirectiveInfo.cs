using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

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

		public abstract ReadOnlyCollection<DirectiveAttributeInfo> SupportedAttributes { get; }

		[CanBeNull]
		public DirectiveAttributeInfo GetAttributeByName([CanBeNull] string attributeName) {
			if (String.IsNullOrEmpty(attributeName))
				return null;
			return SupportedAttributes.FirstOrDefault(di => di.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
		}

		protected DirectiveInfo([NotNull] string name) {
			_name = name;
		}

	}

}