using System;

namespace component.template.api.domain.Models.Common;

public class BaseError
{
    public int? StatusCode { get; set; }
    public string Message { get; set; }
    public string Source { get; set; }
    public string Type { get; set; }
}
