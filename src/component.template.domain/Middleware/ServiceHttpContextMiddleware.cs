using System;
using component.template.domain.Helpers;
using component.template.domain.Models.Common;
using Microsoft.AspNetCore.Http;

namespace component.template.domain.Middleware;

public class ServiceHttpContextMiddleware
{
    private readonly RequestDelegate _next;

    public ServiceHttpContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Código a ser executado antes da DI
        // var headers = context.Request.Headers;
        // var queryParams = context.Request.Query;
        SetContextAccessInfo(context);

        // Log ou manipular os dados conforme necessário
        // Console.WriteLine("Headers: " + headers);
        // Console.WriteLine("Query Params: " + queryParams);

        await _next(context);
    }

    private void SetContextAccessInfo(HttpContext context)
    {
        var contextAccessInfo = new HttpContextAccessInfo()
        {
            Id = long.TryParse(context.User?.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))?.Value, out long id) ? id : null,
            Name = context.User?.Identity?.Name,
            Role = context.User?.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"))?.Value            
        };
        HttpHelper.AddContextItem<HttpContextAccessInfo>(nameof(HttpContextAccessInfo), contextAccessInfo);
    }
}
