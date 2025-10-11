using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace component.template.api.configuration
{
    public static class ServiceExtension
    {
        public static void AddConfiguration(this IServiceCollection services, IGeneralApiConfig config) =>
            config.Initialize(services);

        public static JObject GetApplicationInfo(this IConfiguration configuration)
        {
            var appSection = configuration.GetSection("Application").GetChildren();
            var appConfig = new JObject();

            foreach (var section in appSection)
            {
                appConfig[section.Key] = section.Value;
            }

            return appConfig;
        }

    }
}