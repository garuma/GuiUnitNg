using System;
using NUnit.Framework.Api;

namespace GuiUnitNg
{
	public class GuiRunTestsAction : FrameworkController.FrameworkControllerAction
	{
		public GuiRunTestsAction (FrameworkController controller, string filter, object handler)
		{
			if (TextRunner.MainLoop == null) {
				RealRunTests (controller, filter, handler);
			} else {
				TextRunner.MainLoop.InitializeToolkit ();
				System.Threading.ThreadPool.QueueUserWorkItem (d => {
					try {
						RealRunTests (controller, filter, handler);
					} catch (Exception ex) {
						Console.WriteLine ("Unexpected error while running the tests: {0}", ex);
					} finally {
						TextRunner.Shutdown (1);
					}
				});
				TextRunner.MainLoop.RunMainLoop ();
			}
		}

		object RealRunTests (FrameworkController controller, string filter, object handler)
		{
			return new FrameworkController.RunTestsAction (controller, filter, handler);
		}
	}
}
