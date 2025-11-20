using Aurora.Mediator;

namespace component.template.domain.Models.Internal.User.Queries;

public record GetUserByIdInternalQuery : IRequest<GetUserByIdInternalResponse>
{
    public long Id { get; set; }
}