using System;
using component.template.domain.Models.Common;

namespace component.template.domain.Interfaces.Common;

public interface IApiResponse<T>
{
    public List<BaseError>? Errors { get; set; }
    T? Data { get; set; }
}
