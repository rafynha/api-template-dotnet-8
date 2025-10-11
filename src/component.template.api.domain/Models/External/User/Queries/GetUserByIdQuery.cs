using Aurora.Mediator;

namespace component.template.api.domain.Models.External.User.Queries;

public record GetUserByIdQuery : IRequest<GetUserByIdResponse>
{
    public long Id { get; set; }
}
