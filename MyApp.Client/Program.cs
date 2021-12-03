using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceStack;
using Blazored.LocalStorage;
using Blazor.Extensions.Logging;
using MyApp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddLogging(c => c
    .AddBrowserConsole()
    .SetMinimumLevel(LogLevel.Trace)
);
builder.RootComponents.Add<App>("#app");
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddBlazoredLocalStorage(config =>
    config.JsonSerializerOptions.WriteIndented = true);

// Use / for local or CDN resources
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<ServiceStackStateProvider>());

// Use {ApiBaseUrl} for API requests
builder.Services.AddScoped(_ =>
{
    var client = BlazorClient.Create(builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress);
    client.UseBasePath = "/api";
    return client;
});
builder.Services.AddScoped<ServiceStackStateProvider>();

await builder.Build().RunAsync();
