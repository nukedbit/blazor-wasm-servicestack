using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceStack;

namespace MyApp.Client
{
    public abstract class StackBaseComponent : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }


        [Inject]
        private JsonHttpClient Client {get;set;}

        protected async Task<JsonHttpClient> GetClientAsync() {
            var state = await AuthenticationStateTask;
            if(state.User is ClaimsPrincipal principal && principal.Identity.IsAuthenticated){
                Client.BearerToken = principal.Claims.Where(c => c.Type == "token").FirstOrDefault()?.Value;
            }
            return Client;
        }
    }
}