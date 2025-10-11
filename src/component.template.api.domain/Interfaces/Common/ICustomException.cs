using System.Diagnostics;
using System.Net;

namespace component.template.api.domain.Interfaces.Common;

public interface ICustomException
{
    string DetailsMessage { get; }
    string FaultCode { get; }
    TraceLevel Level { get; }
    HttpStatusCode StatusCode { get; }
}
