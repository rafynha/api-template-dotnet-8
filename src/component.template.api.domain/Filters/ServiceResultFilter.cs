using component.template.api.domain.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace component.template.api.domain.Filters;

public class ServiceResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            if(objectResult.StatusCode == 200 || objectResult.StatusCode == 201)
                objectResult.Value = new DefaultApiResponse<object>{ Data = objectResult.Value };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}

//https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-9.0&viewFallbackFrom=aspnetcore-2.0#result-filters
