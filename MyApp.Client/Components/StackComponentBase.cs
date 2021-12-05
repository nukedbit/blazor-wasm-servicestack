using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ServiceStack;

namespace MyApp.Client;

public abstract class StackComponentBase : ComponentBase
{
    [Inject]
    protected JsonApiClient? Client { get; set; }

    protected async Task<ApiResult<TResponse>> ApiAsync<TResponse>(IReturn<TResponse> request) =>
        await Client!.ApiAsync(request);

    protected async Task<ApiResult<EmptyResponse>> ApiAsync(IReturnVoid request) =>
        await Client!.ApiAsync(request);

    protected async Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> request) =>
        await Client!.SendAsync(request);

    public static string ClassNames(params string?[] classes) => CssUtils.ClassNames(classes);
}
