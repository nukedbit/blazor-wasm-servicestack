# Blazor WASM & ServiceStack for .NET 5

This template integrate ServiceStack with Blazor on .NET 5 (currently on preview).
The blazor wasm app is served through the main ServiceStack web host so you can deploy a single package with both api and client ux.

The blazor example provide a login page with the following default credentials.

    admin@localhost.local
    p@55wOrd
   
Once logged in you can test calling protected endpoints on call hello page.

ServiceStack auth is integrated with blazor on ``ServiceStackStateProvider`` and I have provided a StackBaseComponent to use as a base component in your project, so it provide the JsonHttpClient with the bearer token once authenticated.

Inside the client project you can see this.

            builder.Services.AddScoped<JsonHttpClient>(s =>
            {
                return BlazorClient.Create("https://localhost:5001");
            });

Maybe i will make this automatically get the base url in the future, any contribution is appreciated :).




[![](blazor-wasm-servicestack.png)](https://github.com/nukedbit/blazor-wasm-servicestack)


> Browse [source code](https://github.com/NetCoreTemplates/selfhost), view live demo [selfhost.web-templates.io](http://selfhost.web-templates.io) and install with [dotnet-new](https://docs.servicestack.net/dotnet-new):

    $ dotnet tool install -g x

    $ x new selfhost ProjectName

