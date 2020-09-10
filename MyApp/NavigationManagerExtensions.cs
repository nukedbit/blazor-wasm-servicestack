using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace   MyApp {

    //https://jasonwatmore.com/post/2020/08/09/blazor-webassembly-get-query-string-parameters-with-navigation-manager
    public static class NavigationManagerExtensions
    {        
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            string query = new Uri(navigationManager.Uri).Query;
            return HttpUtility.ParseQueryString(query);
        }

        
        public static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }
    }
}