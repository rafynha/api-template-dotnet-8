using Aurora.Mediator;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Helpers;
using component.template.api.domain.Models.External.User.Commands;

namespace component.template.api.business.Services.User.Validations;

public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public async Task ValidateAsync(CreateUserCommand instance, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(instance.Username))
            throw new RequiredFieldException("Username é obrigatório.");

        if (instance.Username.Length > 50)
            throw new InvalidFieldException("Username deve ter no máximo 50 caracteres.");

        if (string.IsNullOrWhiteSpace(instance.Email))
            throw new RequiredFieldException("Email é obrigatório.");

        if (instance.Email.Length > 100)
            throw new InvalidFieldException("Email deve ter no máximo 100 caracteres.");

        if (!EmailHelper.IsValidEmail(instance.Email))
            throw new InvalidFieldException("Email deve ter um formato válido.");

        if (string.IsNullOrWhiteSpace(instance.Password))
            throw new RequiredFieldException("Password é obrigatório.");

        if (instance.Password.Length < 6)
            throw new InvalidFieldException("Password deve ter no mínimo 6 caracteres.");

        await Task.CompletedTask;
    }
}