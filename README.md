# NetCoreHttpPerfDemo

To see the issue, clone this repo, open the solution in Visual Studio 2017, Update 3 or later. 
Build the solution. Execute the console apps that are built as part of the solution.

The repo consists of three projects:
* **HttpPerfDemo** - a multi-targeted class library that builds a .NET Standard 2.0 binary and a .NET Framework 4.5 binary.
* **HttpPerfExampleTester** - a .NET Core 2.0 console app that references the HttpPerfDemo library.
* **HttpPerfExampleFullFrameworkTester** - a .NET Framework 4.5 console app that also references the HttpPerfDemo library.

Each console app does the following things:
1. Launches chromedriver.exe, which opens a local HTTP server running on port 9515.
2. Starts a session, which launches the Chrome browser via chromedriver.
3. Navigates to the Google home page (this step is immaterial to the demo at hand).
4. Makes 10 HTTP calls to the local server running within chromedriver.exe, logging the elapsed time.
5. Quits the session, which closes the browser, and kills the chromedriver.exe process.

When running via .NET Core, each HTTP call takes at least 1000 milliseconds. When running via the full .NET Framework,
each HTTP call takes less than 100 milliseconds.
