using ServiceStack;

namespace MyApp.Client;

public static class ApiResultExtensions
{
    public static string InvalidClass<T>(this ApiResult<T> apiResult, string fieldName) => apiResult.HasFieldError(fieldName)
        ? "is-invalid"
        : "";
}
