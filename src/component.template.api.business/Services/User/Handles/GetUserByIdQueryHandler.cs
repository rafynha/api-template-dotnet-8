using Aurora.Mediator;
using AutoMapper;
using component.template.api.domain.Common;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.External.User;
using component.template.api.domain.Models.External.User.Queries;
using component.template.api.domain.Models.Internal.Profile.Queries;

namespace component.template.api.business.Services.User.Handles;

public class GetUserByIdQueryHandler : BaseHandler, IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<domain.Models.Repository.UserDto, GetUserByIdResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
    }

    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.FindAsync(x => 
                    x.UserId == request.Id
                );

        var user = users.FirstOrDefault();
        if (user == null)
            throw new DataNotFoundException($"O usuário com id {request.Id} não foi encontrado.");


        // Exemplo chamando outro handler via mediator
        var profile = await _mediator.Send(new GetProfileByIdQuery { Id = request.Id }, cancellationToken);

        // TODO Mapster
        return _mapper.Map<GetUserByIdResponse>(user);
    }
}
