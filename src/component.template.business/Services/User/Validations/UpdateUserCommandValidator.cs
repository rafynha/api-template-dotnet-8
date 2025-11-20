using Aurora.Mediator;
using component.template.domain.Exceptions;
using component.template.domain.Helpers;
using component.template.domain.Models.External.User.Commands;

namespace component.template.business.Services.User.Validations;

public class UpdateUserCommandValidator : IValidator<UpdateUserCommand>
{
    public async Task ValidateAsync(UpdateUserCommand instance, CancellationToken cancellationToken = default)
    {
        if (instance.Id <= 0)
            throw new InvalidFieldException("Id deve ser maior que zero.");

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

        // Password é opcional no update, mas se fornecido deve ser válido
        if (!string.IsNullOrWhiteSpace(instance.Password))
        {
            if (instance.Password.Length < 6)
                throw new InvalidFieldException("Password deve ter no mínimo 6 caracteres.");
        }

        await Task.CompletedTask;
    }
}
