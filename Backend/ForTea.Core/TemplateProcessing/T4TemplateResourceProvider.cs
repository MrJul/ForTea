using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	// TODO: make this shell component
	public class T4TemplateResourceProvider
	{
		[NotNull] private const string InitialBaseClassNamePlaceholder = "TEMPLATE_PLACEHOLDER_BASE_CLASS";
		[NotNull] private const string InitialClassNamePlaceholder = "TEMPLATE_PLACEHOLDER_CLASS";
		[NotNull] private const string FallbackBaseClass = "class " + InitialBaseClassNamePlaceholder + " { }";

		[NotNull]
		private string Template { get; }

		[NotNull]
		private static string ReadTemplate([NotNull] string resourceName, [NotNull] Type caller)
		{
			var assembly = Assembly.GetAssembly(caller);
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null) return FallbackBaseClass;
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		[NotNull]
		public string ProcessResource([CanBeNull] string baseClassName = null, [CanBeNull] string className = null)
		{
			string result = Template;
			if (baseClassName != null) result = result.Replace(InitialBaseClassNamePlaceholder, baseClassName);
			if (className != null) result = result.Replace(InitialClassNamePlaceholder, className);
			return result;
		}

		public T4TemplateResourceProvider([NotNull] string resourceName, [NotNull] object caller) =>
			Template = ReadTemplate(resourceName, caller.GetType());
	}
}
