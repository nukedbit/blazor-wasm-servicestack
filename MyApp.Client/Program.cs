using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceStack;
using Blazored.LocalStorage;
using Blazor.Extensions.Logging;

namespace MyApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddLogging(builder => builder
                .AddBrowserConsole()
                .SetMinimumLevel(LogLevel.Trace)
            );
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddBlazoredLocalStorage(config =>
               config.JsonSerializerOptions.WriteIndented = true);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<ServiceStackStateProvider>());

            builder.Services.AddScoped(_ => BlazorClient.Create(builder.HostEnvironment.BaseAddress));
            builder.Services.AddScoped<ServiceStackStateProvider>();
            

            await builder.Build().RunAsync();
        }
    }
}
