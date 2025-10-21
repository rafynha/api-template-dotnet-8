using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace component.template.api.configuration.General;

public class OpenTelemetryConfig : IGeneralApiConfig
{
    private IConfiguration _configuration { get; }
    public OpenTelemetryConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Initialize(IServiceCollection services)
    {
        var otelConfig = _configuration.GetSection("OpenTelemetry");

        var serviceName = otelConfig["ServiceName"] ?? "unknown-service";
        var environment = otelConfig["Environment"] ?? "development";

        // VALIDAÇÃO DOS ENDPOINTS
        var logEndpoint = GetValidEndpoint(otelConfig["LogEndpoint"]);
        var metricEndpoint = GetValidEndpoint(otelConfig["MetricEndpoint"]);
        var traceEndpoint = GetValidEndpoint(otelConfig["TraceEndpoint"]);

        ActivitySource activitySource = new(serviceName);
        services.AddSingleton(activitySource);

        var openTelemetryBuilder = services.AddOpenTelemetry();

        // CONFIGURAÇÃO DE LOGS (apenas se endpoint for válido)
        if (!string.IsNullOrEmpty(logEndpoint))
        {
            openTelemetryBuilder.WithLogging(logsBuilder =>
            {
                logsBuilder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName)
                            .AddAttributes(new[] {
                                new KeyValuePair<string, object>("deployment.environment", environment),
                                new KeyValuePair<string, object>("version", "1.0.0")
                            })
                    )
                    // .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(logEndpoint);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
            });
        }

        // CONFIGURAÇÃO DE TRACES E METRICS
        if (!string.IsNullOrEmpty(traceEndpoint))
        {
            openTelemetryBuilder.WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(serviceName)
                                .AddAttributes(new[] {
                                    new KeyValuePair<string, object>("deployment.environment", environment),
                                    new KeyValuePair<string, object>("version", "1.0.0")
                                })
                        )
                        .AddSource(activitySource.Name)
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.EnrichWithException = (activity, exception) =>
                            {
                                activity.SetTag("stacktrace", exception.StackTrace);
                            };
                        })
                        .AddHttpClientInstrumentation()
                        // .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(traceEndpoint);
                            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        });
                }
            );
        }

        if (!string.IsNullOrEmpty(metricEndpoint))
        {
            openTelemetryBuilder.WithMetrics(meterProviderBuilder =>
                {
                    meterProviderBuilder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(serviceName)
                                .AddAttributes(new[] {
                                    new KeyValuePair<string, object>("deployment.environment", environment),
                                    new KeyValuePair<string, object>("version", "1.0.0")
                                })
                        )
                        .AddMeter(activitySource.Name)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        // .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(metricEndpoint);
                            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        });
                }
            );
        }        

        // LOG DE CONFIRMAÇÃO
        var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger<OpenTelemetryConfig>();

        logger?.LogInformation(
            "OpenTelemetry configurado - Service: {ServiceName}, Environment: {Environment}\n" +
            "Logs: {LogStatus}, Metrics: {MetricStatus}, Traces: {TraceStatus}",
            serviceName,
            environment,
            string.IsNullOrEmpty(logEndpoint) ? "❌ DISABLED" : $"✅ ENABLED ({logEndpoint})",
            string.IsNullOrEmpty(metricEndpoint) ? "❌ DISABLED" : $"✅ ENABLED ({metricEndpoint})",
            string.IsNullOrEmpty(traceEndpoint) ? "❌ DISABLED" : $"✅ ENABLED ({traceEndpoint})"
        );
    }

    // MÉTODO PARA VALIDAR ENDPOINTS
    private string? GetValidEndpoint(string? endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            return null;

        // Remove espaços e verifica se não está vazio
        endpoint = endpoint.Trim();
        if (string.IsNullOrEmpty(endpoint))
            return null;

        // Verifica formato básico de URL
        if (!endpoint.StartsWith("http://") && !endpoint.StartsWith("https://"))
        {
            // Tenta adicionar http:// se faltar
            endpoint = "http://" + endpoint;
        }

        return endpoint;
    }
}