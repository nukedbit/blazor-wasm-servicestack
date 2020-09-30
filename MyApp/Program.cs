using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Funq;
using Microsoft.Extensions.Configuration;
using ServiceStack;
using MyApp.ServiceInterface;
using MyApp.ServiceModel;

namespace MyApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   var env = hostingContext.HostingEnvironment;
                   config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                   config.AddEnvironmentVariables();
               })
               .ConfigureWebHostDefaults(webBuilder =>
               {

                   webBuilder.UseModularStartup<Startup, StartupActivator>();
               });
    }

    public class StartupActivator : ModularStartupActivator
    {
        public StartupActivator(IConfiguration configuration) : base(configuration)
        {

        }
    }

    public class Startup : ModularStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseServiceStack(new AppHost());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
            });

        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost()
            : base("MyApp", typeof(MyServices).Assembly) {
                  typeof(Hello).AddAttributes(new AuthenticateAttribute());
            }

        public override void Configure(Container container)
        {
            Plugins.Add(new CorsFeature(allowedHeaders: "Content-Type,Authorization"));


        }
    }
}
