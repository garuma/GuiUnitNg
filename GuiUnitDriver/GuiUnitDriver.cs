// ***********************************************************************
// Copyright (c) 2009-2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Engine.Extensibility;
using System.Runtime.Serialization;
using NUnit.Engine;

namespace GuiUnitDriver
{
	/// <summary>
	/// NUnitFrameworkDriver is used by the test-runner to load and run
	/// tests using the NUnit framework assembly.
	/// </summary>
	public class GuiUnitDriver : IFrameworkDriver
	{
		private const string GuiUnit = "GuiUnit";
		private const string LOAD_MESSAGE = "Method called without calling Load first";

		private static readonly string CONTROLLER_TYPE = "NUnit.Framework.Api.FrameworkController";
		private static readonly string LOAD_ACTION = CONTROLLER_TYPE + "+LoadTestsAction";
		private static readonly string EXPLORE_ACTION = CONTROLLER_TYPE + "+ExploreTestsAction";
		private static readonly string COUNT_ACTION = CONTROLLER_TYPE + "+CountTestsAction";
		private static readonly string RUN_ACTION = "GuiUnitNg.GuiRunTestsAction";
		private static readonly string STOP_RUN_ACTION = CONTROLLER_TYPE + "+StopRunAction";

		AppDomain _testDomain;
		string _testAssemblyPath;

		object _frameworkController;

		/// <summary>
		/// Construct an NUnit3FrameworkDriver
		/// </summary>
		/// <param name="testDomain">The AppDomain in which to create the FrameworkController</param>
		public GuiUnitDriver (AppDomain testDomain)
		{
			_testDomain = testDomain;
		}

		public string ID { get; set; }

		/// <summary>
		/// Loads the tests in an assembly.
		/// </summary>
		/// <returns>An Xml string representing the loaded test</returns>
		public string Load (string testAssemblyPath, IDictionary<string, object> settings)
		{
			settings ["InternalTraceLevel"] = "Debug";
			GuiUnitDriverFactory.Log (string.Format ("Driver > Load {0} with settings {1}", testAssemblyPath,
			                                         string.Join (", ", settings.Select (s => s.ToString ()))));
			var idPrefix = string.IsNullOrEmpty (ID) ? "" : ID + "-";
			_testAssemblyPath = testAssemblyPath;
			try {
				_frameworkController = CreateObject (CONTROLLER_TYPE, testAssemblyPath, idPrefix, settings);
			} catch (SerializationException ex) {
				throw new NUnitEngineException ("The NUnit 3.0 driver cannot support this test assembly. Use a platform specific runner.", ex);
			}

			GuiUnitDriverFactory.Log ("Driver > Load ControllerLoaded");

			CallbackHandler handler = new CallbackHandler ();

			var fileName = Path.GetFileName (_testAssemblyPath);

			CreateObject (LOAD_ACTION, _frameworkController, handler);

			GuiUnitDriverFactory.Log ("Driver > Load CallbackReceived " + handler.Result);

			return handler.Result;
		}

		public int CountTestCases (string filter)
		{
			GuiUnitDriverFactory.Log (string.Format ("Driver > CountTestCases {0}", filter));
			CheckLoadWasCalled ();

			CallbackHandler handler = new CallbackHandler ();

			CreateObject (COUNT_ACTION, _frameworkController, filter, handler);

			return int.Parse (handler.Result);
		}

		/// <summary>
		/// Executes the tests in an assembly.
		/// </summary>
		/// <param name="listener">An ITestEventHandler that receives progress notices</param>
		/// <param name="filter">A filter that controls which tests are executed</param>
		/// <returns>An Xml string representing the result</returns>
		public string Run (ITestEventListener listener, string filter)
		{
			CheckLoadWasCalled ();

			CallbackHandler handler = new RunTestsCallbackHandler (listener);

			CreateObject (RUN_ACTION, _frameworkController, filter, handler);

			return handler.Result;
		}

		/// <summary>
		/// Cancel the ongoing test run. If no  test is running, the call is ignored.
		/// </summary>
		/// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
		public void StopRun (bool force)
		{
			CreateObject (STOP_RUN_ACTION, _frameworkController, force, new CallbackHandler ());
		}

		/// <summary>
		/// Returns information about the tests in an assembly.
		/// </summary>
		/// <param name="filter">A filter indicating which tests to include</param>
		/// <returns>An Xml string representing the tests</returns>
		public string Explore (string filter)
		{
			CheckLoadWasCalled ();

			CallbackHandler handler = new CallbackHandler ();

			CreateObject (EXPLORE_ACTION, _frameworkController, filter, handler);

			return handler.Result;
		}

		#region Helper Methods

		private void CheckLoadWasCalled ()
		{
			if (_frameworkController == null)
				throw new InvalidOperationException (LOAD_MESSAGE);
		}

		private object CreateObject (string typeName, params object [] args)
		{
			return _testDomain.CreateInstanceAndUnwrap (
				GuiUnit, typeName, false, 0, null, args, null, null
			);
		}

		#endregion
	}
}