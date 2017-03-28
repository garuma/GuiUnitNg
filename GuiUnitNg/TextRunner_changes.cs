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
				return RunTests (filter, runSettings);
			} else {
				MainLoop.InitializeToolkit ();
				int result = UNEXPECTED_ERROR;
				System.Threading.ThreadPool.QueueUserWorkItem (d => {
					try {
						result = RunTests (filter, runSettings);
					} catch (Exception ex) {
						Console.WriteLine ("Unexpected error while running the tests: {0}", ex);
					} finally {
						Shutdown ();
					}
				});
				MainLoop.RunMainLoop ();
				return result;
			}
		}

		internal static void Shutdown ()
		{
			// Run the shutdown method on the main thread
			var helper = new InvokerHelper {
				Func = () => { MainLoop.Shutdown (); return null; }
			};
			MainLoop.InvokeOnMainLoop (helper);
		}

		public void OtherTestStarted (ITest test)
		{
			foreach (var l in extraListeners)
				l.TestStarted (test);
		}

		public void OtherTestFinished (ITestResult result)
		{
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
