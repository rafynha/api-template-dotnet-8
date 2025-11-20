using System;
using System.Net;
using component.template.domain.Interfaces.Common;
using Newtonsoft.Json;

namespace component.template.domain.Exceptions;

public class UnexpectedErrorException : Exception, ICustomException
{
    private const string customMessage = "Unexpected error!";

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

    public UnexpectedErrorException()
        : base(customMessage)
    { }

    public UnexpectedErrorException(string message)
        : base(message)
    { }

    public UnexpectedErrorException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public UnexpectedErrorException(Exception innerException)
        : base(customMessage, innerException)
    { }

    public UnexpectedErrorException(object message)
      : base($"{customMessage} --> {JsonConvert.SerializeObject(message)}")
    { }
}
