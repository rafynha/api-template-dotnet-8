using System;
using System.Net;
using component.template.api.domain.Interfaces.Common;

namespace component.template.api.domain.Exceptions;

public class DataNotFoundException : System.Exception, ICustomException
{
    private const string customMessage = "Data not found.";

    public string DetailsMessage
    {
        get
        {
            return customMessage;
        }
    }

    public string FaultCode
    {
        get
        {
            return "BadRequest";
        }
    }

    public System.Diagnostics.TraceLevel Level
    {
        get
        {
            return System.Diagnostics.TraceLevel.Warning;
        }
    }

    public HttpStatusCode StatusCode
    {
        get
        {
            return HttpStatusCode.BadRequest;
        }
    }

    public DataNotFoundException()
        : base(customMessage)
    { }

    public DataNotFoundException(string message)
        : base(message)
    { }

    public DataNotFoundException(string message, System.Exception innerException)
        : base(message, innerException)
    { }

    public DataNotFoundException(System.Exception innerException)
        : base(customMessage, innerException)
    { }

    public DataNotFoundException(object message)
      : base($"{customMessage} --> {System.Text.Json.JsonSerializer.Serialize(message)}")
    { }
}