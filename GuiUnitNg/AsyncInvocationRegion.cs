using System;

namespace NUnit.Framework.Internal
{
	// Plug our logging in
	internal abstract class AsyncInvocationRegion : IDisposable
	{
		private AsyncInvocationRegion ()
		{
			GuiUnitNg.GuiUnitEventSource.Log.AsyncInvocationStart ();
		}

		public virtual void Dispose ()
		{
			GuiUnitNg.GuiUnitEventSource.Log.AsyncInvocationStop ();
		}
	}
}
