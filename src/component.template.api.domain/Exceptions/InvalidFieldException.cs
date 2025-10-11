using System.Net;
using component.template.api.domain.Interfaces.Common;

namespace component.template.api.domain.Exceptions;

public class InvalidFieldException : ArgumentException, ICustomException
{
    private const string customMessage = "Invalid field value";
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

    public InvalidFieldException() : base(customMessage)
    {
    }

    public InvalidFieldException(string message) : base(message)
    {
    }

    public InvalidFieldException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InvalidFieldException(string message, string paramName) : base(message, paramName)
    {
    }

    public InvalidFieldException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
    {
    }

    public InvalidFieldException(string message, string paramName, int line, int column) : base(message, paramName)
    {
    }

}
