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
    private AuthenticateResponse authenticationResponse;
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
            if (authenticationResponse is null)
            {
                authenticationResponse = await LocalStorage.GetItemAsync<AuthenticateResponse>("Authentication");
            }
            if (authenticationResponse is null)
            {
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            List<Claim> claims = new()
            {
                new Claim("token", authenticationResponse.BearerToken, ClaimValueTypes.String, null),
                new Claim(ClaimTypes.NameIdentifier, authenticationResponse.SessionId),
                new Claim(ClaimTypes.Name, authenticationResponse.UserName),
                new Claim(ClaimTypes.Email, authenticationResponse.UserName)
            };
            foreach (var role in authenticationResponse.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            foreach (var permission in authenticationResponse.Permissions)
            {
                claims.Add(new Claim("perm", permission, ClaimValueTypes.String, null));
            }
            identity = new ClaimsIdentity(claims, "Server authentication");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Request failed:" + ex.ToString());
            await LocalStorage.RemoveItemAsync("Authentication");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("Authentication");
        try
        {
            await client.PostAsync(new Authenticate()
            {
                provider = "logout"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        authenticationResponse = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<AuthenticateResponse> Login(string email, string password)
    {
        authenticationResponse = await client.PostAsync(new Authenticate
        {
            provider = "credentials",
            Password = password,
            UserName = email,
            UseTokenCookie = true
        });
        await LocalStorage.SetItemAsync("Authentication", authenticationResponse);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return authenticationResponse;
    }
}