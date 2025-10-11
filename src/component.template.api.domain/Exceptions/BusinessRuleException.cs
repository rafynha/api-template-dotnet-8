using System;
using System.Net;
using component.template.api.domain.Interfaces.Common;

namespace component.template.api.domain.Exceptions;

public class BusinessRuleException : System.Exception, ICustomException
{
    private const string customMessage = "Business rule violation.";

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

    public BusinessRuleException()
        : base(customMessage)
    { }

    public BusinessRuleException(string message)
        : base(message)
    { }

    public BusinessRuleException(string message, System.Exception innerException)
        : base(message, innerException)
    { }

    public BusinessRuleException(System.Exception innerException)
        : base(customMessage, innerException)
    { }

    public BusinessRuleException(object message)
      : base($"{customMessage} --> {System.Text.Json.JsonSerializer.Serialize(message)}")
    { }
}