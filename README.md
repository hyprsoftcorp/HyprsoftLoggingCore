# HyprsoftLogging
A very simple logging framework.  While developing .NET Core 2.x apps for Windows IoT Core we ran into runtime issues using the file loggers provided by NLog and Serilog.  Unfortunately we were forced to write our own logging framework.

## Architecture
The architecture of this framework borrows concepts from both NLog and Serilog but is extremely lightweight with minimal functionality (on purpose).

### Loggers
1. SimpleDebugLogger - Writes log entries to the debug window.  Enabled by default.
2. SimpleConsoleLogger - Writes log entries to the console.
3. SimpleFileLogger - Writes log entries to a file which allows you to specify a MaxFileSizeBytes and MaxArchiveFileCount to manage log file size, log history, disk space consumption, etc.

## Sample Code
This sample code configures the log manager to write log entires to the debug window and a log file located in the same folder as the executing assembly.  Log files will be archived once the log file size exceeds 500k and only the 3 most recent archived log files will be kept.
```csharp
var logger = new SimpleLogManager();
logger.AddLogger(new SimpleFileLogger(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "myapp-log.log"))
{
	MaxFileSizeBytes = 524288,
	MaxArchiveFileCount = 3
});
await logger.LogAsync<LoggerTests>(LogLevel.Info, "Hello world!");
```

### Output
```
INFO:	Hyprsoft.Logging.Tests.LoggerTests @ 12/24/2018 9:20:15 AM
	Hellow world!
```
