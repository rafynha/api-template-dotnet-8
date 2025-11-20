using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using component.template.domain.Exceptions;

namespace component.template.configuration.General;

public class AuthenticationConfiguration : IGeneralApiConfig
{
    private IConfiguration _configuration { get; }
    public AuthenticationConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Initialize(IServiceCollection services)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new ConfigurationNotFoundExceptionException("JwtSettings:Secret"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudiences = jwtSettings.GetSection("Audience").Get<string[]>(),
                    ValidateLifetime = true
                };
            });        
    }
}