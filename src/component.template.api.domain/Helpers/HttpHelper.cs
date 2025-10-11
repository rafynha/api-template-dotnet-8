using Microsoft.AspNetCore.Http;

namespace component.template.api.domain.Helpers;

public static class HttpHelper
{
    private static IHttpContextAccessor _accessor;
    public static HttpContext HttpContext => _accessor.HttpContext;
    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _accessor = httpContextAccessor;
    }

    public static void AddContextItem<T>(string name, T item) where T : class
    {
        if(HttpHelper.HttpContext.Items[name] == null)
            HttpHelper.HttpContext.Items.Add(name,item);
    }
}
