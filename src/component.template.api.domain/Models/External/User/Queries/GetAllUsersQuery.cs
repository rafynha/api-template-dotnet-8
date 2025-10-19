using Aurora.Mediator;
using component.template.api.domain.Models.Common;

namespace component.template.api.domain.Models.External.User.Queries;

public record GetAllUsersQuery : IRequest<PagedResult<GetAllUsersResponse>>
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}