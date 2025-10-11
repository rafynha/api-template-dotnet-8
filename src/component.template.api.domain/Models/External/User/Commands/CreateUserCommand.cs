using Aurora.Mediator;

namespace component.template.api.domain.Models.External.User.Commands;

public class CreateUserCommand : IRequest<CreateUserResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}