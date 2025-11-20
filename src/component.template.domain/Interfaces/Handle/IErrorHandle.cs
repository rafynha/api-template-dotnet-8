using component.template.domain.Models.Common;

namespace component.template.domain.Interfaces.Handle;

public interface IErrorHandle
{
    List<BaseError> Errors { get; set; }
    void AddError(Exception error, int? statusCode = null);
}
