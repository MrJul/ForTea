using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	public class T4TemplateBaseProvider
	{
		[NotNull] private const string InitialClassNamePlaceholder = "TemplateBase";
		[NotNull] private const string FallbackBaseClass = "class " + InitialClassNamePlaceholder + " {{ }}";

		[NotNull]
		private string BaseClassFormat { get; }

		[NotNull]
		private static string ReadBaseClass([NotNull] string resourceName, [NotNull] Type caller)
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
		public string CreateTemplateBase([CanBeNull] string baseClassName) =>
			BaseClassFormat.Replace(InitialClassNamePlaceholder, baseClassName);

		public T4TemplateBaseProvider([NotNull] string resourceName, [NotNull] object caller) =>
			BaseClassFormat = ReadBaseClass(resourceName, caller.GetType());
	}
}
