using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Logging;

namespace Hyprsoft.Logging.TestWeb
{
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
}
