using System.Linq;
using System.Reflection;

namespace GuiUnit
{
	public class LoadFileHelper
	{
		internal static MethodInfo LoadFileMethod;

		static LoadFileHelper ()
		{
			LoadFileMethod = typeof (Assembly).GetMethods ().FirstOrDefault (m => {
				return m.Name == "LoadFile" && m.GetParameters ().Length == 1 && m.GetParameters () [0].ParameterType == typeof (string);
			});
		}
	}
}
