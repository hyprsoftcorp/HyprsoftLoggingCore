using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hyprsoft.Logging.TestWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSingleton<FakeService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseWelcomePage();

            serviceProvider.GetService<FakeService>().DoSomething();

            /* Controller constructor injection example.
            public HomeController(ILogger<HomeController> logger) 
            (
                logger.LogDebug($"{nameof(HomeController)} ctor.");
            )
            */
        }
    }
}
