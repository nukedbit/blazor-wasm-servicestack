[![](blazor-wasm-servicestack.png)](https://github.com/nukedbit/blazor-wasm-servicestack)

# Blazor WASM & ServiceStack for .NET 5

This template integrate ServiceStack with Blazor on .NET 5 and ServiceStack 5.9.3 (currently on preview).
The blazor wasm app is served through the main ServiceStack web host so you can deploy a single package with both api and client ux.

## Quick Start 

Use the x tool to install this template

    $ dotnet tool install -g x

    $ x new nukedbit/blazor-wasm-servicestack ProjectName

This template come with sqlite db preconfigured so it work out of the box.


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

## Code Sharing

Right now I have got dto sharing by using ``MyApp.ServiceModel`` project with success as suggested on [Physical Project Structure](https://docs.servicestack.net/physical-project-structure)

## Running your app

There are two launch profiles for VSCode, the firt one it will launch the app with blazor debugging, the other one it will launch only the WebHost Debugging.

1) Launch and Debug Blazor WebAssembly
2) .NET Core Launch (web)


If you don't know ServiceStack give it a try it come with a AGPL 3 license, but you can get a commercial license for cheap and it will make your day more productive and enjoyable :) 

Check the following links for more info

## References

* https://docs.servicestack.net 
* https://servicestack.net 



