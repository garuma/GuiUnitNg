using System;
using System.Collections.Generic;
using GuiUnit;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

/*** Changes guide:
 * This partial declaration implements two method groups referenced from the upstream file:
 * 
 * - The first is simply RunTestsWrapper to allow calling RunTests on a worker thread
 * - The other group are OtherTest* methods following ITestListener to be able to executed our own listeners
 * 
 * Apart from the necessary callsites introduced in the upstream version of this partial class, nothing
 * else there should be changed and any other modification that is introduced should be in this file.
 */
namespace GuiUnitNg
{
	public partial class TextRunner
	{
		static bool initialized = false;
		static IMainLoopIntegration mainLoop;
		List<ITestListener> extraListeners = new List<ITestListener> ();

		internal bool WantsOverallTestCount { get; set; }
		internal int OverallTestCount { get; set; }

		public void AddExtraListener (ITestListener listener)
		{
			if (listener == null)
				throw new ArgumentNullException (nameof (listener));
			extraListeners.Add (listener);
		}

		public static IMainLoopIntegration MainLoop {
			get {
				if (initialized)
					return mainLoop;

				initialized = true;
				try { mainLoop = mainLoop ?? new XwtMainLoopIntegration (); } catch { }
				try { mainLoop = mainLoop ?? new MonoMacMainLoopIntegration (); } catch { }
				try { mainLoop = mainLoop ?? new GtkMainLoopIntegration (); } catch { }
				return mainLoop;
			}
			set {
				mainLoop = value;
			}
		}

		public int RunTestsWrapper (TestFilter filter, IDictionary<string, object> runSettings)
		{
			if (MainLoop == null) {
				return Math.Min (255, RunTests (filter, runSettings));
			} else {
				MainLoop.InitializeToolkit ();
				int result = UNEXPECTED_ERROR;
				System.Threading.ThreadPool.QueueUserWorkItem (d => {
					try {
						result = Math.Min (255, RunTests (filter, runSettings));
					} catch (Exception ex) {
						Console.WriteLine ("Unexpected error while running the tests: {0}", ex);
					} finally {
						Shutdown (result);
					}
				});
				MainLoop.RunMainLoop ();
				return result;
			}
		}

		internal static void Shutdown (int exitCode)
		{
			// Run the shutdown method on the main thread
			var helper = new InvokerHelper {
				Func = () => { MainLoop.Shutdown (exitCode); return null; }
			};
			MainLoop.InvokeOnMainLoop (helper);
		}

		public void OtherTestStarted (ITest test)
		{
			switch (test) {
			case TestMethod testMethod:
				GuiUnitNg.GuiUnitEventSource.Log.TestMethodStart (testMethod.FullName);
				break;
			case TestSuite testSuite:
				GuiUnitNg.GuiUnitEventSource.Log.TestSuiteStart (testSuite.FullName);
				break;
			}

			foreach (var l in extraListeners)
				l.TestStarted (test);
		}

		public void OtherTestFinished (ITestResult result)
		{
			switch (result.Test) {
			case TestMethod testMethod:
				GuiUnitNg.GuiUnitEventSource.Log.TestMethodStop ();
				break;
			case TestSuite testSuite:
				GuiUnitNg.GuiUnitEventSource.Log.TestSuiteStop ();
				break;
			}

			foreach (var l in extraListeners)
				l.TestFinished (result);
		}

		public void OtherTestOutput (TestOutput output)
		{
			foreach (var l in extraListeners)
				l.TestOutput (output);
		}
	}
}
