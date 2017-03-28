using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using GuiUnit;

namespace GuiUnitNg
{
	class MainClass : TextRunner
	{
		public static int Main (string [] args)
		{
			var tcpListener = MaybeCreateTcpListener (args);
			args = PreProcessArgs (args);
			var runner = new GuiUnitNg.TextRunner ();
			if (tcpListener != null)
				runner.AddExtraListener (tcpListener);
			return runner.Execute (args);
		}

		/* The below methods allow the runner to be used out of the box with Xamarin Studio
		 * NUnit integration by understanding the older version of the command-line it
		 * uses and maintaining a compatible NUnit 2 XML output
		 */
		static string [] PreProcessArgs (string [] args)
		{
			return args.Select (a => {
				// Modify some arguments for compatibility with XS NUnit addin
				if (a.StartsWith ("-run", StringComparison.OrdinalIgnoreCase))
					return "-test" + a.Substring ("-run".Length);
				if (a.StartsWith ("-xml", StringComparison.OrdinalIgnoreCase))
					return "-result" + a.Substring ("-xml".Length) + ";format=nunit2";
				if (a.StartsWith ("-port", StringComparison.OrdinalIgnoreCase))
					return null;
				return a;
			}).Where (a => !string.IsNullOrEmpty (a)).ToArray ();
		}

		static ITestListener MaybeCreateTcpListener (string [] args)
		{
			var portArg = args.FirstOrDefault (a => a.StartsWith ("-port", StringComparison.OrdinalIgnoreCase));
			if (portArg == null)
				return null;
			int port;
			if (!int.TryParse (portArg.Substring ("-port=".Length), out port))
				return null;
			var writer = new TcpWriter ("localhost", port);
			var listener = new XmlTestListener (writer);
			Console.WriteLine ("Setup TCP listener on port: " + port);
			return listener;
		}
	}
}
