using Aurora.Mediator;
using AutoMapper;
using component.template.api.domain.Common;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.Internal.User;
using component.template.api.domain.Models.Internal.User.Queries;
using component.template.api.domain.Models.Repository;

namespace component.template.api.business.Services.User.Handles;

public class GetUserByIdInternalQueryHandler : BaseHandler, IRequestHandler<GetUserByIdInternalQuery, GetUserByIdInternalResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdInternalQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<UserDto, GetUserByIdInternalResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }

    public async Task<GetUserByIdInternalResponse> Handle(GetUserByIdInternalQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.FindAsync(x => x.UserId == request.Id);

        var user = users.FirstOrDefault();
        if (user == null)
            throw new DataNotFoundException($"O usuário com id {request.Id} não foi encontrado.");

        return _mapper.Map<GetUserByIdInternalResponse>(user);
    }
}