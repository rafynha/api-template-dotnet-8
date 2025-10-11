using Aurora.Mediator;

namespace component.template.api.domain.Models.Internal.Profile.Queries;

public record GetProfileByIdQuery : IRequest<GetProfileByIdResponse>
{
    public long Id { get; set; }
}