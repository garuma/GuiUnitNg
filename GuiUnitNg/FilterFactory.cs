using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace GuiUnitNg
{
	public static class FilterFactory
	{
		public static TestFilter CreateFullNameFilter (string test)
		{
			var fullNameFilterType = typeof (TestFilter).Assembly.GetType ("NUnit.Framework.Internal.Filters.FullNameFilter");
			return (TestFilter)Activator.CreateInstance (fullNameFilterType, test);
		}

		public static TestFilter CreateOrFilter (ITestFilter[] filters)
		{
			var orFilterType = typeof (TestFilter).Assembly.GetType ("NUnit.Framework.Internal.Filters.OrFilter");
			return (TestFilter)Activator.CreateInstance (orFilterType, filters);
		}

		public static TestFilter CreateAndFilter (ITestFilter filter1, ITestFilter filter2)
		{
			var andFilterType = typeof (TestFilter).Assembly.GetType ("NUnit.Framework.Internal.Filters.AndFilter");
			return (TestFilter)Activator.CreateInstance (andFilterType, filter1, filter2);
		}
	}
}
