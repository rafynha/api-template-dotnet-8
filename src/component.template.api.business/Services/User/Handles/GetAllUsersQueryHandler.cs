using Aurora.Mediator;
using AutoMapper;
using component.template.api.domain.Common;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.Common;
using component.template.api.domain.Models.External.User;
using component.template.api.domain.Models.External.User.Queries;

namespace component.template.api.business.Services.User.Handles;

public class GetAllUsersQueryHandler : BaseHandler, IRequestHandler<GetAllUsersQuery, PagedResult<GetAllUsersResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<domain.Models.Repository.UserDto, GetAllUsersResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }

    public async Task<PagedResult<GetAllUsersResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // Construir a expressão de filtro
        System.Linq.Expressions.Expression<Func<domain.Models.Repository.UserDto, bool>> filter = x =>
            (string.IsNullOrEmpty(request.Username) || x.Username.Contains(request.Username)) &&
            (string.IsNullOrEmpty(request.Email) || x.Email.Contains(request.Email)) &&
            (!request.IsActive.HasValue || x.IsActive == request.IsActive.Value);

        // Buscar dados paginados (Skip/Take aplicado no banco) e contar em paralelo
        // OrderBy por UserId descendente
        var (pagedUsers, totalCount) = await _unitOfWork.Users.FindPagedAsync(
            predicate: filter, 
            pageNumber: request.PageNumber, 
            pageSize: request.PageSize,
            orderBy: x => x.UserId,
            orderByDescending: true
        );

        // Mapear para o response
        var mappedUsers = _mapper.Map<IEnumerable<GetAllUsersResponse>>(pagedUsers);

        // Retornar resultado paginado
        return new PagedResult<GetAllUsersResponse>(mappedUsers, totalCount, request.PageNumber, request.PageSize);
    }
}