using System.Net;
using component.template.api.domain.Interfaces.Common;

namespace component.template.api.domain.Exceptions;

public class RequiredFieldException : ArgumentException, ICustomException
{
    private const string customMessage = "Required field is missing";
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

    public RequiredFieldException() : base(customMessage)
    {
    }

    public RequiredFieldException(string message) : base(message)
    {
    }

    public RequiredFieldException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public RequiredFieldException(string message, string paramName) : base(message, paramName)
    {
    }

    public RequiredFieldException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
    {
    }

    public RequiredFieldException(string message, string paramName, int line, int column) : base(message, paramName)
    {
    }

}
