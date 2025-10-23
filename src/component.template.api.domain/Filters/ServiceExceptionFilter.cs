using System.Diagnostics;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Interfaces.Common;
using component.template.api.domain.Interfaces.Handle;
using component.template.api.domain.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace component.template.api.domain.Filters;

#region Web References 
// https://nwb.one/blog/exception-filter-attribute-dotnet
#endregion
public class ServiceExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<ServiceExceptionFilter> _logger;
    private readonly IErrorHandle _errorHandle;

    public ServiceExceptionFilter(ILogger<ServiceExceptionFilter> logger, IErrorHandle errorHandle)
    {
        this._logger = logger;
        this._errorHandle = errorHandle;
    }

    public override void OnException(ExceptionContext context)
    {
        // CAPTURA o TraceId da requisição atual
        var traceId = Activity.Current?.TraceId.ToString() ??
                     context.HttpContext.TraceIdentifier;
                     
        var customException = ConvertException(context.Exception);
        AddErrorToResult(context, customException);
        
        var result = new ObjectResult(new DefaultApiResponse<object>(_errorHandle.Errors))
        {
            StatusCode = (int)customException.StatusCode
        };

        // ADICIONA TraceId no header HTTP
        context.HttpContext.Response.Headers.TryAdd("X-Trace-Id", traceId);

        // Log the exception
        LogExceptionByLevel(context.Exception, customException.Level, traceId);

        // Set the result
        context.Result = result;
    }

    private void LogExceptionByLevel(Exception exception, TraceLevel level, string traceId)
    {
        switch (level)
        {
            case TraceLevel.Warning:
                _logger.LogWarning("Exception occurred while executing request: {ex}, TraceId: {traceId}", exception, traceId);
                break;
            case TraceLevel.Error:
            default:
                _logger.LogError("Exception occurred while executing request: {ex}, TraceId: {traceId}", exception, traceId);
                break;
        }
    }

    private void AddErrorToResult(ExceptionContext context, ICustomException customException)
    {
        _errorHandle.Errors.Add(new()
        {
            Message = context.Exception.Message, // Or a different generic message
            StatusCode = (int)customException.StatusCode,//(int)statusCode,
            Source = context.Exception.Source,
            Type = context.Exception.GetType().FullName
        });
    }

    private ICustomException ConvertException(Exception ex)
    {
        var customException = default(ICustomException);
        var knownTypes = new Type[]
        {
            // typeof(CallServiceErrorException),
            // typeof(DataNotFoundException),
            // typeof(ExistingDataException),
            // typeof(NullParameterException),
            // typeof(ProcessErrorException),
            // typeof(UnauthorizedException),
            typeof(InvalidFieldException),
            typeof(RequiredFieldException),  
            typeof(InvalidModelStateException),
            typeof(ConfigurationNotFoundExceptionException)
        };

        if (knownTypes.Contains(ex.GetType()))
        {
            customException = (ICustomException)ex;
        }
        else
        {
            var newException = new UnexpectedErrorException(ex.Message, ex);
            customException = (ICustomException)newException;
        }

        return customException;
    }
}
