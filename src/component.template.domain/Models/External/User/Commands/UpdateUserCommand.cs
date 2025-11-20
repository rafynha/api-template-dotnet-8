using Aurora.Mediator;

namespace component.template.domain.Models.External.User.Commands;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool IsActive { get; set; }
}
