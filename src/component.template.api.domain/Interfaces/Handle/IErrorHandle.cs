using component.template.api.domain.Models.Common;

namespace component.template.api.domain.Interfaces.Handle;

public interface IErrorHandle
{
    List<BaseError> Errors { get; set; }
    void AddError(Exception error, int? statusCode = null);
}
