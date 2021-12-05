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
