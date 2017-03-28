using System;
using System.Reflection;
using NUnit.Framework.Internal;

namespace GuiUnit
{
	public class InvokerHelper
	{
		internal object Result;
		internal Func<object> Func;
		internal Exception ex;
		internal TestExecutionContext Context;
		internal System.Threading.ManualResetEvent Waiter = new System.Threading.ManualResetEvent(false);

		PropertyInfo currentContextProperty;

		public void Invoke ()
		{
			SetCurrentTestExecutionContext (Context);
			try {
				Result = Func ();
			} catch (Exception e) {
				ex = e;
			} finally {
				Waiter.Set ();
			}
		}

		void SetCurrentTestExecutionContext (TestExecutionContext context)
		{
			if (currentContextProperty == null)
				currentContextProperty = typeof (TestExecutionContext).GetProperty (nameof (TestExecutionContext.CurrentContext));
			currentContextProperty.SetValue (null, context);
		}
	}
}

