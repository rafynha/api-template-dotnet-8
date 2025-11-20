using System;
using component.template.configuration.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace component.template.configuration.General;

public class SwaggerConfiguration : IGeneralApiConfig
{
    private IConfiguration _configuration { get; }
    public SwaggerConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void Initialize(IServiceCollection services)
    {
        var appConfig = _configuration.GetApplicationInfo();

        services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = appConfig["Name"]?.ToString() ?? "API NAME",
                    Version = appConfig["Version"]?.ToString() ?? "vX",
                    Description = appConfig["Description"]?.ToString() ?? "Api description",
                    Contact = new OpenApiContact()
                    {
                        Name = "",
                        Email = "teste@email.com",
                        Url = new Uri(appConfig["Site"]?.ToString() ?? string.Empty)
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Copyright Template Api BApps © 2025",
                        Url = new Uri("https://license.com/")
                    }
                });

                // Configurar o esquema de segurança JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta forma: Bearer {seu token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                c.DocumentFilter<RemoveInternalSchemasFilter>();
            });
    }
}