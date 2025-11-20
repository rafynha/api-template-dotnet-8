using Aurora.Mediator;
using AutoMapper;
using component.template.domain.Common;
using component.template.domain.Exceptions;
using component.template.domain.Helpers;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.domain.Models.External.User;
using component.template.domain.Models.External.User.Commands;
using component.template.domain.Models.Repository;

namespace component.template.business.Services.User.Handles;

public class CreateUserCommandHandler : BaseHandler, IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<CreateUserCommand, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => PasswordHelper.HashPassword(src.Password)))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        cfg.CreateMap<UserDto, CreateUserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Validar se username já existe
            var existingUsersByUsername = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
            if (existingUsersByUsername.Any())
            {
                throw new BusinessRuleException("Username já está em uso.");
            }

            // Validar se email já existe
            var existingUsersByEmail = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existingUsersByEmail.Any())
            {
                throw new BusinessRuleException("Email já está em uso.");
            }

            // Criar novo usuário
            var userDto = _mapper.Map<UserDto>(request);
            
            // Adicionar ao repositório
            await _unitOfWork.Users.AddAsync(userDto);
            await _unitOfWork.CommitAsync();

            await _unitOfWork.CommitTransactionAsync();

            // Retornar resposta
            return _mapper.Map<CreateUserResponse>(userDto);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}