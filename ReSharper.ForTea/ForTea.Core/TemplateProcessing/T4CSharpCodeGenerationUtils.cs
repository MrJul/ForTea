using System;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	public static class T4CSharpCodeGenerationUtils
	{
		public const string DefaultTargetExtension = "cs";

		[NotNull]
		public static string WithExtension([NotNull] this string name, [NotNull] string newExtension)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (newExtension == null) throw new ArgumentNullException(nameof(newExtension));
			int dotIndex = name.LastIndexOf('.');

			if (dotIndex < 0) return name + newExtension;

			return name.Substring(0, dotIndex + 1) + newExtension;
		}

		/// <returns>
		/// Target extension. Leading dot, if any, is removed.
		/// </returns>
		public static string GetTargetExtension(
			[NotNull] this IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));
			if (manager == null) throw new ArgumentNullException(nameof(manager));

			OutputDirectiveInfo output = manager.Output;
			var query =
				from outputDirective in file.GetDirectives(output)
				from attribute in outputDirective.GetAttributes()
				let name = attribute.GetName()
				where string.Equals(name, output.ExtensionAttribute.Name, StringComparison.OrdinalIgnoreCase)
				select attribute.GetValue();
			string targetExtension = query.FirstOrDefault();

			if (targetExtension == null) return DefaultTargetExtension;

			return targetExtension.StartsWith(".", StringComparison.Ordinal)
				? targetExtension.Substring(1)
				: targetExtension;
		}
	}
}
