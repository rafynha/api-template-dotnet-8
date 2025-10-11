using System;
using component.template.api.domain.Models.Common;

namespace component.template.api.domain.Interfaces.Common;

public interface IApiResponse<T>
{
    public List<BaseError>? Errors { get; set; }
    T? Data { get; set; }
}
