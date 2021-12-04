using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;
using ServiceStack;

namespace MyApp.Client;

public static class AppExtensions
{
    //https://jasonwatmore.com/post/2020/08/09/blazor-webassembly-get-query-string-parameters-with-navigation-manager
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        string query = new Uri(navigationManager.Uri).Query;
        return HttpUtility.ParseQueryString(query);
    }

    public static string? QueryString(this NavigationManager navigationManager, string key) =>
        navigationManager.QueryString()[key];

    public static string GetReturnUrl(this NavigationManager navigationManager)
    {
        var returnUrl = navigationManager.QueryString("return");
        if (returnUrl == null || returnUrl.IsEmpty())
            return "/";
        return returnUrl;
    }

    public static string InvalidClass<T>(this ApiResult<T> apiResult, string fieldName) =>
        apiResult.ErrorStatus.InvalidClass(fieldName);

    public static string InvalidClass(this ResponseStatus? status, string fieldName) =>
        CssUtils.Bootstrap.InvalidClass(status, fieldName);

}
