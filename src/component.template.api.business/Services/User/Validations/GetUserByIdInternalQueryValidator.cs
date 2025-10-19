using Aurora.Mediator;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Models.Internal.User.Queries;

namespace component.template.api.business.Services.User.Validations;

public class GetUserByIdInternalQueryValidator : IValidator<GetUserByIdInternalQuery>
{
    public async Task ValidateAsync(GetUserByIdInternalQuery instance, CancellationToken cancellationToken = default)
    {
        if (instance.Id <= 0)
            throw new InvalidFieldException("Invalid user ID.");

        await Task.CompletedTask;
    }
}