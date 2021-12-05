using Funq;
using ServiceStack;
using MyApp.ServiceInterface;

namespace MyApp;

public class AppHost : AppHostBase
{
    public AppHost() : base("MyApp", typeof(MyServices).Assembly) { }

    public override void Configure(Container container)
    {
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
    }
}
