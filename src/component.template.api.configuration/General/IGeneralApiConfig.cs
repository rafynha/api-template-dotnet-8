using Microsoft.Extensions.DependencyInjection;

namespace component.template.api.configuration
{
    public interface IGeneralApiConfig
    {
        void Initialize(IServiceCollection services);
    }
}