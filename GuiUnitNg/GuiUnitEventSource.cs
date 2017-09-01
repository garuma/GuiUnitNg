using System;
using System.Diagnostics.Tracing;

namespace GuiUnitNg
{
	[EventSource (Name = "GuiUnitNg-Test")]
	public class GuiUnitEventSource : EventSource
	{
		public static GuiUnitEventSource Log = new GuiUnitEventSource ();

		public void TestRunnerStart () => WriteEvent (1);

		public void TestRunnerStop () => WriteEvent (2);

		public void TestMethodStart (string testName) => WriteEvent (3, testName);

		public void TestMethodStop () => WriteEvent (4);

		public void TestSuiteStart (string testSuiteName) => WriteEvent (5, testSuiteName);

		public void TestSuiteStop () => WriteEvent (6);

		public void InnerInvokeStart (string testName, string methodName, string fixtureName) => WriteEvent (7, testName, methodName, fixtureName);

		public void InnerInvokeStop () => WriteEvent (8);

		public void AsyncInvocationStart () => WriteEvent (9);

		public void AsyncInvocationStop () => WriteEvent (10);
	}
}
