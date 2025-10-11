using System;
using System.Net;
using component.template.api.domain.Interfaces.Common;
using Newtonsoft.Json;

namespace component.template.api.domain.Exceptions;

public class InvalidModelStateException: Exception, ICustomException
{
    private const string customMessage = "Invalid Model State";

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
            return System.Diagnostics.TraceLevel.Error;
        }
    }

    public HttpStatusCode StatusCode
    {
        get
        {
            return HttpStatusCode.BadRequest;
        }
    }

    public InvalidModelStateException()
        : base(customMessage)
    { }

    public InvalidModelStateException(string message)
        : base(message)
    { }

    public InvalidModelStateException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public InvalidModelStateException(Exception innerException)
        : base(customMessage, innerException)
    { }

    public InvalidModelStateException(object message)
      : base($"{customMessage} --> {JsonConvert.SerializeObject(message)}")
    { }
}
