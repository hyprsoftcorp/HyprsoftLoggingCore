using Hyprsoft.Logging.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Hyprsoft.Logging.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Using Dependency Injection

            var services = new ServiceCollection();
            var serviceProvider = services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddSimpleFileLogger();
                builder.AddConsole();
            })
            .AddSingleton<FakeService>()
            .BuildServiceProvider();

            var service = serviceProvider.GetService<FakeService>();
            service.DoSomething();

            #endregion

            #region Not using Dependency Injection

            var factory = new LoggerFactory();
            factory.AddSimpleFileLogger();
            factory.AddConsole();

            var logger = factory.CreateLogger<Program>();
            logger.LogInformation("Hello World!");

            #endregion

            Console.ReadKey();
        }
    }
}
