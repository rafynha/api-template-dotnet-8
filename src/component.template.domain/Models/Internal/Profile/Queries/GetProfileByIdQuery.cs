using Aurora.Mediator;

namespace component.template.domain.Models.Internal.Profile.Queries;

public record GetProfileByIdQuery : IRequest<GetProfileByIdResponse>
{
    public long Id { get; set; }
}