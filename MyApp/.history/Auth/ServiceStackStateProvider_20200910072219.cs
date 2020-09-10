using System.Net.Http;
using System.Net.Http.Json
using Microsoft.AspNetCore.Components.Forms
using Microsoft.AspNetCore.Components.Routing
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace   MyApp {
    public class CustomStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService api;
    private CurrentUser _currentUser;
    public CustomStateProvider(IAuthService api)
    {
        this.api = api;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        try
        {
            var userInfo = await GetCurrentUser();
            if (userInfo.IsAuthenticated)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }.Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));
                identity = new ClaimsIdentity(claims, "Server authentication");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Request failed:" + ex.ToString());
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    private async Task<CurrentUser> GetCurrentUser()
    {
        if (_currentUser != null && _currentUser.IsAuthenticated) return _currentUser;
        _currentUser = await api.CurrentUserInfo();
        return _currentUser;
    }
    public async Task Logout()
    {
        await api.Logout();
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public async Task Login(LoginRequest loginParameters)
    {
        await api.Login(loginParameters);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public async Task Register(RegisterRequest registerParameters)
    {
        await api.Register(registerParameters);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
}