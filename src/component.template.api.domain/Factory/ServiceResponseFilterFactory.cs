using System;
using component.template.api.domain.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace component.template.api.domain.Factory;

public class ResponseFilterFactory : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) =>
        new InternalResponseFilter();

    private class InternalResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.StatusCode == 200)
                    objectResult.Value = new DefaultApiResponse<object> { Data = objectResult.Value };
            }
        }
    }
}
