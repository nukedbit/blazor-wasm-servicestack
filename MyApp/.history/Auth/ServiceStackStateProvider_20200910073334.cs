using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using ServiceStack;

namespace MyApp
{
    public class ServiceStackStateProvider : AuthenticationStateProvider
    {
        private AuthenticateResponse _authenticationResponse;
        private readonly JsonHttpClient client;

        public ServiceStackStateProvider(JsonHttpClient client)
        {
            this.client = client;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            if(_authenticationResponse is null){
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                    }
            try
            {
                var userInfo = await GetCurrentUser();
                if (userInfo.IsAuthenticated)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }.Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "Server authentication");
                }
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, _authenticationResponse.UserName));
                claims.Add(new Claim(ClaimTypes.Email, _authenticationResponse.UserName));
                
                claims.Add(new Claim(ClaimTypes.Role, _authenticationResponse.UserName));
                
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task Logout()
        {
            await client.PostAsync(new Authenticate() {
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
            }); 
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return _authenticationResponse;
        }
    }
}