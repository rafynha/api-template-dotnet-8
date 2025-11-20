using Microsoft.Extensions.DependencyInjection;

namespace component.template.configuration
{
    public interface IGeneralApiConfig
    {
        void Initialize(IServiceCollection services);
    }
}