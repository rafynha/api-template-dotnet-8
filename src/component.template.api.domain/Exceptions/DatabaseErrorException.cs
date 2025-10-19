using System;
using System.Net;
using component.template.api.domain.Interfaces.Common;

namespace component.template.api.domain.Exceptions;

public class DatabaseErrorException : System.Exception, ICustomException
{
    private const string customMessage = "Database operation error.";

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
            return "InternalServerError";
        }
    }

    public System.Diagnostics.TraceLevel Level
    {
        get
        {
            return System.Diagnostics.TraceLevel.Error;
        }
    }

    public HttpStatusCode StatusCode
    {
        get
        {
            return HttpStatusCode.InternalServerError;
        }
    }

    public DatabaseErrorException()
        : base(customMessage)
    { }

    public DatabaseErrorException(string message)
        : base(message)
    { }

    public DatabaseErrorException(string message, System.Exception innerException)
        : base(message, innerException)
    { }

    public DatabaseErrorException(System.Exception innerException)
        : base(customMessage, innerException)
    { }

    public DatabaseErrorException(object message)
      : base($"{customMessage} --> {System.Text.Json.JsonSerializer.Serialize(message)}")
    { }
}