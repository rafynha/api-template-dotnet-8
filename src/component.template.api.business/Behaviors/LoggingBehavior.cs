using System;
using Aurora.Mediator;
using Microsoft.Extensions.Logging;

namespace component.template.api.business.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            _logger.LogInformation($"➡️ Handling {typeof(TRequest).Name} with data: {(request != null ? System.Text.Json.JsonSerializer.Serialize(request) : "null")}");

            var response = await next();

            _logger.LogInformation($"✅ Handled {typeof(TRequest).Name}. Response: {System.Text.Json.JsonSerializer.Serialize(response)}");

            return response;
        }
        catch (System.Exception ex)
        {
            _logger.LogWarning($"❌ An error occurred while handling {typeof(TRequest).Name}: {ex.Message}");
            throw;
        }
    }
}