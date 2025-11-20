using Aurora.Mediator;
using component.template.domain.Exceptions;
using component.template.domain.Models.Internal.User.Queries;

namespace component.template.business.Services.User.Validations;

public class GetUserByIdInternalQueryValidator : IValidator<GetUserByIdInternalQuery>
{
    public async Task ValidateAsync(GetUserByIdInternalQuery instance, CancellationToken cancellationToken = default)
    {
        if (instance.Id <= 0)
            throw new InvalidFieldException("Invalid user ID.");

        await Task.CompletedTask;
    }
}