using Aurora.Mediator;
using FluentValidation;

namespace component.template.api.business.Behaviors;

public class ValidationBehaviorWithFluent<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<FluentValidation.IValidator<TRequest>> _validators;

    public ValidationBehaviorWithFluent(IEnumerable<FluentValidation.IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
