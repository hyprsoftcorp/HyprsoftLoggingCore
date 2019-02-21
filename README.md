# Simple File Logger
A very simple file logging framework based on the Microsoft.Extensions.Logging.Abstractions NuGet package.  While developing .NET Core 2.x apps for Windows IoT Core we ran into runtime issues using the file loggers provided by NLog and Serilog.  Unfortunately we were forced to write our own file logger.
The SimpleFileLogger writes log entries to a file which allows you to specify a MaxFileSizeBytes and MaxArchiveFileCount to manage log file size, log history, disk space consumption, etc.

### Logger Options
Name | Description | Default
--- | --- | ---
RootFolder | Log file folder name. | Entry assembly folder.
Filename | Filename of the log file. | app-log.log
LogLevel | Logger log level. | LogLevel.Information
MaxFileSizeBytes | The maximum file size in bytes before the log file is archived. | 0 - no archiving will occur.
MaxArchiveFileCount | The number of archive files to retain. | 5

## Sample Code
### Console Application (DI)
```csharp
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        var serviceProvider = services.AddLogging(builder =>
        {
            builder.AddSimpleFileLogger();
            builder.AddConsole();
        })
        .AddSingleton<FakeService>()
        .BuildServiceProvider();

        var service = serviceProvider.GetService<FakeService>();
        service.DoSomething();

        Console.ReadKey();
    }
}
```
### Console Application
```csharp
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Logging;
using System;

class Program
{
    static void Main(string[] args)
    {
        var factory = new LoggerFactory();
        factory.AddSimpleFileLogger();
        factory.AddConsole();

        var logger = factory.CreateLogger<Program>();
        logger.LogInformation("Hello World!");

        Console.ReadKey();
    }
}
```

### ASP.NET Core MVC Web App
```csharp
using Microsoft.AspNetCore.Hosting;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.AddSimpleFileLogger();
                builder.AddFilter<SimpleFileLoggerProvider>("Microsoft", LogLevel.None);
            })
            .UseStartup<Startup>();
}
```

### Sample Output
```
INFO: Hyprsoft.Logging.TestWeb.FakeService @ 2/20/2019 5:40:54 PM
	Hello World!
```
