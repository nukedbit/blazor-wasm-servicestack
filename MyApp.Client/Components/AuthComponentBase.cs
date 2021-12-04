using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace MyApp.Client;

public abstract class AuthComponentBase : StackComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected bool HasInit { get; set; }

    protected bool IsAuthenticated { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var state = await AuthenticationStateTask!;
        IsAuthenticated = state.User?.Identity?.IsAuthenticated ?? false;
        HasInit = true;
    }
}

/// <summary>
/// Required to enable CORS requests
/// </summary>
public class EnableCorsMessageHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}