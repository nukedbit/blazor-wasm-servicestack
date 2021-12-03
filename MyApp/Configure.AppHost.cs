using Funq;
using ServiceStack;
using MyApp.ServiceInterface;

[assembly: HostingStartup(typeof(MyApp.AppHost))]

namespace MyApp;

public class AppHost : AppHostBase, IHostingStartup
{
    public AppHost() : base("MyApp", typeof(MyServices).Assembly) { }

    public override void Configure(Container container)
    {
        RawHttpHandlers.Add(ApiHandlers.Json("/api/{Request}"));

        SetConfig(new HostConfig
        {
        });

        Plugins.Add(new CorsFeature(allowedHeaders: "Content-Type,Authorization",
            allowOriginWhitelist: new[]{
            "https://localhost:5002",
            "https://localhost:44344",
            "http://localhost:56811",
            "https://{DEPLOY_CDN}"
        }, allowCredentials: true));

        Plugins.Add(new AutoQueryDataFeature()); // For TodosService
    }

    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
            services.ConfigureNonBreakingSameSiteCookies(context.HostingEnvironment));
}
