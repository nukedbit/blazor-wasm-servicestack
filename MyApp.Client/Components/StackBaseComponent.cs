using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceStack;

namespace MyApp.Client;

public abstract class StackBaseComponent : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    [Inject]
    private JsonHttpClient? Client { get; set; }

    protected async Task<JsonHttpClient> GetClientAsync()
    {
        var state = await AuthenticationStateTask!;
        if (state.User is ClaimsPrincipal principal && principal.Identity?.IsAuthenticated == true)
        {
            Client!.BearerToken = principal.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
        }
        return Client!;
    }

    protected async Task<ApiResult<TResponse>> ApiAsync<TResponse>(IReturn<TResponse> request) =>
        await (await GetClientAsync()).ApiAsync(request);

    protected async Task<ApiResult<EmptyResponse>> ApiAsync(IReturnVoid request) =>
        await (await GetClientAsync()).ApiAsync(request);

    protected async Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> request) =>
        await (await GetClientAsync()).SendAsync(request);

    public static string ClassNames(params string[] classes) => CssUtils.ClassNames(classes);
}
