using System.Linq;
using System.Security.Claims;
using System.Text;
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

    public static string ClassNames(params string[] classes)
    {
        var sb = new StringBuilder();
        foreach (var cls in classes)
        {
            if (cls.IsNullOrEmpty())
                continue;

            if (sb.Length > 0)
                sb.Append(' ');
            sb.Append(cls);
        }
        return sb.ToString();
    }
}
