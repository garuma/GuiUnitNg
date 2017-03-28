using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Text;
using NUnit.Framework.Interfaces;

namespace GuiUnit
{
	public class XmlTestListener : ITestListener
	{
		ThreadLocal<StringBuilder> output = new ThreadLocal<StringBuilder> (() => new StringBuilder ());

		TextWriter Writer {
			get; set;
		}

		public XmlTestListener (TextWriter writer)
		{
			Writer = writer;
		}

		public void TestStarted (ITest test)
		{
			output.Value.Clear ();
			if (test.HasChildren)
				Write (new XElement ("suite-started", new XAttribute ("name", test.FullName)));
			else
				Write (new XElement ("test-started", new XAttribute ("name", test.FullName)));
		}

		public void TestFinished (ITestResult result)
		{
			var element = new XElement (result.Test.HasChildren ? "suite-finished" : "test-finished",
			                            new XAttribute ("name", result.Test.FullName),
			                            new XAttribute ("result", ToXmlString (result.ResultState)),
			                            new XAttribute ("passed", result.PassCount),
			                            new XAttribute ("failures", result.FailCount),
			                            new XAttribute ("ignored", result.SkipCount),
			                            new XAttribute ("inconclusive", result.InconclusiveCount)
			);
			if (!string.IsNullOrEmpty (result.Message))
				element.Add (new XAttribute ("message", result.Message));
			if (!string.IsNullOrEmpty (result.StackTrace))
				element.Add (new XAttribute ("stack-trace", result.StackTrace));
			if (output.Value.Length > 0)
				element.Add (new XAttribute ("output", output.Value.ToString ()));
			Write (element);
		}

		public void TestOutput (TestOutput testOutput)
		{
			output.Value.Append (testOutput);
		}

		object ToXmlString (ResultState resultState)
		{
			if (resultState == ResultState.Success)
				return "Success";
			else if (resultState == ResultState.Inconclusive)
				return "Inconclusive";
			else if (resultState == ResultState.Ignored)
				return "Ignored";
			else
				return "Failure";
		}

		void Write (XElement element)
		{
			try {
				lock (Writer)
					Writer.WriteLine (element.ToString ());
			} catch {
			}
		}
	}
}

