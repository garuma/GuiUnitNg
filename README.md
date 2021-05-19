# GuiUnitNg

A more up-to-date version of https://github.com/mono/guiunit

This is a small project to compile both a runner and NUnit framework (both based on latest NUnit NuGet package) into a single .exe assembly that can be linked into a test project and used directly to run it.

Like the original GuiUnit, the goal of the runner is to provide integration with a GUI runloop (Gtk, Cocoa, Xwt, ...) so that unit tests that depend on it (for proper async/await operations for instance) can be run correctly.

The verion of this package follows the version of the NUnit dependency it derives from.

Building:
msbuild /r /m /bl /p:Configuration=Release GuiUnitNg.sln

NuGet package will be at:
GuiUnitNg\bin\Release\Xamarin.GuiUnitNg.3.12.0.nupkg

Upload to:
https://www.nuget.org/packages/Xamarin.GuiUnitNg