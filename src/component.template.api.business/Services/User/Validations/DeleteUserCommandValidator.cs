using Aurora.Mediator;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Models.External.User.Commands;

namespace component.template.api.business.Services.User.Validations;

public class DeleteUserCommandValidator : IValidator<DeleteUserCommand>
{
    public async Task ValidateAsync(DeleteUserCommand instance, CancellationToken cancellationToken = default)
    {
        if (instance.Id <= 0)
            throw new InvalidFieldException("Invalid user ID.");

        await Task.CompletedTask;
    }
}