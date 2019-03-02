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
            factory.AddSimpleFileLogger(LogLevel.Trace);
            factory.AddConsole(LogLevel.Trace);

            var logger = factory.CreateLogger<Program>();
            logger.LogCritical("Critical");
            logger.LogDebug("Debug");
            try
            {
                throw new InvalidOperationException("Uh oh...something bad happened.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
            }
            logger.LogInformation("Information");
            logger.LogTrace("Trace");
            logger.LogWarning("Waning");

            #endregion

            Console.ReadKey();
        }
    }
}
