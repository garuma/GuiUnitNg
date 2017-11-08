using System;
using System.IO;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace GuiUnit
{
	public class VstsPercentageTestListener : ITestListener
	{
		GuiUnitNg.TextRunner runner;
		TextWriter rawConsole;
		int testFinished;

		public VstsPercentageTestListener (GuiUnitNg.TextRunner runner, TextWriter rawConsole)
		{
			this.runner = runner;
			this.rawConsole = rawConsole;
		}

		int CurrentPercentage (int finished) => Math.Max (0, Math.Min (100, (int)Math.Floor ((finished * 100d) / runner.OverallTestCount)));

		public void TestFinished (ITestResult result)
		{
			if (result.Test.IsSuite)
				return;
			var currentTestFinished = Interlocked.Increment (ref testFinished);
			rawConsole.WriteLine ("##vso[task.setprogress value={0};]Test {1} finished",
			                      CurrentPercentage (currentTestFinished).ToString (),
			                      result.FullName);
		}

		// Unused
		public void TestOutput (TestOutput output)
		{
		}

		public void TestStarted (ITest test)
		{
		}
	}
}
