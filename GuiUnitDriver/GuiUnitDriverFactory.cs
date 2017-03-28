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

		public GuiUnitDriverFactory ()
		{
			Log ("GuiUnit > ctor");
		}

		public IFrameworkDriver GetDriver (AppDomain domain, AssemblyName reference)
		{
			Log ("GuiUnit > GetDriver");
			return new GuiUnitDriver (domain);
		}

		public bool IsSupportedTestFramework (AssemblyName reference)
		{
			Log ("GuiUnit > IsSupportedTestFramework " + reference.ToString ());
			return reference.Name.IndexOf ("GuiUnit", StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}
