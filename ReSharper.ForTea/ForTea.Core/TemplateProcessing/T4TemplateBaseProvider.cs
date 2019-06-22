using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	public class T4TemplateBaseProvider
	{
		[NotNull] private const string InitialClassNamePlaceholder = "TemplateBase";

		[NotNull]
		private string BaseClassFormat { get; }

		[NotNull]
		private static string ReadBaseClass([NotNull] string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null) return $"class {InitialClassNamePlaceholder} {{ }}";
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		[NotNull]
		public string CreateTemplateBase([CanBeNull] string baseClassName) =>
			BaseClassFormat.Replace(InitialClassNamePlaceholder, baseClassName);

		public T4TemplateBaseProvider([NotNull] string resourceName) => BaseClassFormat = ReadBaseClass(resourceName);
	}
}
