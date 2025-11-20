using AutoMapper;
using component.template.domain.Helpers;
using component.template.domain.Models.Common;

namespace component.template.domain.Common;

public abstract class BaseHandler
{
    public HttpContextAccessInfo _contextAccessInfo => HttpHelper.HttpContext.Items[nameof(HttpContextAccessInfo)] as HttpContextAccessInfo;
    public IMapper _mapper { 
        get 
        {
            return new MapperConfiguration(cfg => ConfigureMappings(cfg)).CreateMapper();
        } 
    }
    public abstract void ConfigureMappings(IMapperConfigurationExpression cfg);  
}