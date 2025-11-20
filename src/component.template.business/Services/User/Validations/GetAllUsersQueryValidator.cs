using Aurora.Mediator;
using component.template.domain.Exceptions;
using component.template.domain.Models.External.User.Queries;

namespace component.template.business.Services.User.Validations;

public class GetAllUsersQueryValidator : IValidator<GetAllUsersQuery>
{
    public async Task ValidateAsync(GetAllUsersQuery instance, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(instance.Username) && instance.Username.Length > 50)
            throw new InvalidFieldException("Username deve ter no máximo 50 caracteres.");

        if (!string.IsNullOrEmpty(instance.Email) && instance.Email.Length > 100)
            throw new InvalidFieldException("Email deve ter no máximo 100 caracteres.");

        if (instance.PageNumber <= 0)
            throw new InvalidFieldException("PageNumber deve ser maior que zero.");

        if (instance.PageSize <= 0)
            throw new InvalidFieldException("PageSize deve ser maior que zero.");

        if (instance.PageSize > 100)
            throw new InvalidFieldException("PageSize não pode ser maior que 100.");

        await Task.CompletedTask;
    }
}