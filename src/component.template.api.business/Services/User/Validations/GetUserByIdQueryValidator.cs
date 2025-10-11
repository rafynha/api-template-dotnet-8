using Aurora.Mediator;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Models.External.User.Queries;

namespace component.template.api.business.Services.User.Validations;

public class GetUserByIdQueryValidator : IValidator<GetUserByIdQuery>
{
    public async Task ValidateAsync(GetUserByIdQuery instance, CancellationToken cancellationToken = default)
    {
        if (instance.Id <= 0)
            throw new InvalidFieldException("Invalid user ID.");

        await Task.CompletedTask;
    }
}
