using component.template.api.domain.Interfaces.Common;

namespace component.template.api.domain.Models.Common;

public class DefaultApiResponse<T> : IApiResponse<T> where T : class
{
    public List<BaseError>? Errors { get; set; } = new();
    public T? Data { get; set; }

    public DefaultApiResponse(T data)
    {
        this.Errors = new();
        this.Data = data;
    }

    public DefaultApiResponse()
    {
        this.Errors = new();
        this.Data = default;
    } 

    public DefaultApiResponse(List<BaseError> errors)
    {
        this.Errors = errors;
        this.Data = null;
    } 

     public static DefaultApiResponse<T> Error(List<BaseError> errors) =>
        new DefaultApiResponse<T>(errors);  
}