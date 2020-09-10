using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using ServiceStack;

namespace MyApp
{
    public class ServiceStackStateProvider : AuthenticationStateProvider
    {
        private AuthenticateResponse _authenticationResponse;
        private readonly JsonHttpClient client;

        
        private ProtectedLocalStorage ProtectedLocalStorage { get; set; }

        public ServiceStackStateProvider(JsonHttpClient client, ProtectedLocalStorage ProtectedLocalStorage)
        {
            this.client = client;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            try
            {
                if (_authenticationResponse is null)
                {
                    var fromSession = await ProtectedLocalStorage.GetAsync<AuthenticateResponse>("Authentication");
                    if (fromSession.Success)
                    {
                        _authenticationResponse ??= fromSession.Value;
                    }
                }
                if (_authenticationResponse is null)
                {
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("token", _authenticationResponse.BearerToken, ClaimValueTypes.String, null));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, _authenticationResponse.SessionId));
                claims.Add(new Claim(ClaimTypes.Name, _authenticationResponse.UserName));
                claims.Add(new Claim(ClaimTypes.Email, _authenticationResponse.UserName));
                foreach (var role in _authenticationResponse.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                foreach (var permission in _authenticationResponse.Permissions)
                {
                    claims.Add(new Claim("perm", permission, ClaimValueTypes.String, null));
                }
                identity = new ClaimsIdentity(claims, "Server authentication");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task Logout()
        {
            await client.PostAsync(new Authenticate()
            {
                provider = "logout"
            });
            _authenticationResponse = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task<AuthenticateResponse> Login(string email, string password)
        {
            _authenticationResponse = await client.PostAsync(new Authenticate()
            {
                provider = "credentials",
                Password = password,
                UserName = email,
                UseTokenCookie = true
            });
            await ProtectedLocalStorage.SetAsync("Authentication", _authenticationResponse);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return _authenticationResponse;
        }
    }
}