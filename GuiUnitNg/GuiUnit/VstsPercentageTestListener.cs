using System;
using System.IO;
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

		int CurrentPercentage => Math.Max (0, Math.Min (100, (int)((testFinished * 100d) / runner.OverallTestCount)));

		public void TestFinished (ITestResult result)
		{
			testFinished++;
			rawConsole.WriteLine ("##vso[task.setprogress value={0};]Test {1} finished",
			                      CurrentPercentage.ToString (),
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
