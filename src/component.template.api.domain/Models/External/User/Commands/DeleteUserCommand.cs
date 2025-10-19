using Aurora.Mediator;

namespace component.template.api.domain.Models.External.User.Commands;

public class DeleteUserCommand : IRequest<DeleteUserResponse>
{
    public long Id { get; set; }
}