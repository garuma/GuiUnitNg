using System;
using System.IO;
using System.Reflection;
using NUnit.Engine.Extensibility;

namespace GuiUnitDriver
{
	[Extension]
	public class GuiUnitDriverFactory : IDriverFactory
	{
		static readonly string LogPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "guiunit-log.txt");

		public static void Log (string message)
		{
			File.AppendAllText (LogPath, message + Environment.NewLine);
		}

		public IFrameworkDriver GetDriver (AppDomain domain, AssemblyName reference)
		{
			return new GuiUnitDriver (domain);
		}

		public bool IsSupportedTestFramework (AssemblyName reference)
		{
			return reference.Name.IndexOf ("GuiUnit", StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}
