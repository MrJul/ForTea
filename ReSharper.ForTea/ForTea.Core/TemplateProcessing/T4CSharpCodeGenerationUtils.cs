using System;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	public static class T4CSharpCodeGenerationUtils
	{
		[NotNull] public const string CSharpExtension = "cs";
		[NotNull] public const string PreprocessResultExtension = "cs";
		[NotNull] public const string DefaultTargetExtension = PreprocessResultExtension;

		[NotNull]
		// name is NOT supposed to contain extension
		public static string WithExtension([NotNull] this string name, [NotNull] string extension)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (extension == null) throw new ArgumentNullException(nameof(extension));
			return name + '.' + extension;
		}
		
		[NotNull]
		// name is supposed to contain extension
		public static string WithOtherExtension([NotNull] this string name, [NotNull] string newExtension)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (newExtension == null) throw new ArgumentNullException(nameof(newExtension));
			return name.WithoutExtension().WithExtension(newExtension);
		}

		[NotNull]
		public static string WithoutExtension([NotNull] this string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			int dotIndex = name.LastIndexOf('.');
			return dotIndex < 0 ? name : name.Substring(0, dotIndex);
		}

		[NotNull]
		public static string EscapeKeyword([NotNull] this string s)
		{
			if (!CSharpLexer.IsKeyword(s)) return s;
			return '@' + s;
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

		[CanBeNull]
		public static string ChooseBetterNamespace([CanBeNull] string first, [CanBeNull] string second)
		{
			if (first == null) return second;
			if (second == null) return first;
			return first.IsEmpty() ? second : first;
		}
	}
}
