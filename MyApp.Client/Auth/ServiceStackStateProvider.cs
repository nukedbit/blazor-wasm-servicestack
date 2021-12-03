using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceStack;

namespace MyApp.Client;

public class ServiceStackStateProvider : AuthenticationStateProvider
{
    private ApiResult<AuthenticateResponse> authResult = new();
    private readonly JsonHttpClient client;

    ILocalStorageService LocalStorage { get; set; }

    public ServiceStackStateProvider(JsonHttpClient client, ILocalStorageService localStorage)
    {
        this.client = client;
        this.LocalStorage = localStorage;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();

        try
        {
            var authResponse = authResult.Completed
                ? authResult.Response
                : await LocalStorage.GetItemAsync<AuthenticateResponse>("Authentication");
            
            if (authResponse is null)
                return new AuthenticationState(new ClaimsPrincipal(identity));

            List<Claim> claims = new()
            {
                new Claim("token", authResponse.BearerToken, ClaimValueTypes.String, null),
                new Claim(ClaimTypes.NameIdentifier, authResponse.SessionId),
                new Claim(ClaimTypes.Name, authResponse.UserName),
                new Claim(ClaimTypes.Email, authResponse.UserName)
            };
            foreach (var role in authResponse.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            foreach (var permission in authResponse.Permissions)
            {
                claims.Add(new Claim("perm", permission, ClaimValueTypes.String, null));
            }

            identity = new ClaimsIdentity(claims, "Server authentication");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Request failed:" + ex.ToString());
            await LocalStorage.RemoveItemAsync("Authentication");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<ApiResult<AuthenticateResponse>> Logout()
    {
        await LocalStorage.RemoveItemAsync("Authentication");
        authResult = await client.ApiAsync(new Authenticate { provider = "logout" });
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return authResult;
    }

    public async Task<ApiResult<AuthenticateResponse>> Login(string email, string password)
    {
        authResult = await client.ApiAsync(new Authenticate
        {
            provider = "credentials",
            Password = password,
            UserName = email,
            UseTokenCookie = true
        });

        if (authResult.IsSuccess)
        {
            await LocalStorage.SetItemAsync("Authentication", authResult.Response!);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        return authResult;
    }
}