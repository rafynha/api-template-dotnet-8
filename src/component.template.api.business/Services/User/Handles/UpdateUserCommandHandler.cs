using Aurora.Mediator;
using AutoMapper;
using component.template.api.domain.Common;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Helpers;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.External.User;
using component.template.api.domain.Models.External.User.Commands;
using component.template.api.domain.Models.Internal.User;
using component.template.api.domain.Models.Internal.User.Queries;
using component.template.api.domain.Models.Repository;

namespace component.template.api.business.Services.User.Handles;

public class UpdateUserCommandHandler : BaseHandler, IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<UpdateUserCommand, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.Password) ? PasswordHelper.HashPassword(src.Password) : null))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        cfg.CreateMap<UserDto, UpdateUserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        cfg.CreateMap<GetUserByIdInternalResponse, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Buscar o UserDto para atualização usando a query interna
            var existingUser = await _mediator.Send(new GetUserByIdInternalQuery { Id = request.Id }, cancellationToken);

            // Validar se username já existe (exceto para o próprio usuário)       
            await ValidateUsername(request, existingUser, _unitOfWork);

            // Validar se email já existe (exceto para o próprio usuário)
            await ValidateEmail(request, existingUser, _unitOfWork);

            // Atualizar dados do usuário
            existingUser.Username = request.Username;
            existingUser.Email = request.Email;
            existingUser.IsActive = request.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;
            ValidateAndUpdatePassword(request, existingUser);

            var user = _mapper.Map<UserDto>(existingUser);

            // Atualizar no repositório
            if (await _unitOfWork.Users.UpdateAsync(user) <= 0)
                throw new DatabaseErrorException("Erro ao atualizar credenciais do usuário.");

            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<UpdateUserResponse>(user);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static void ValidateAndUpdatePassword(UpdateUserCommand request, GetUserByIdInternalResponse existingUser)
    {
        if(string.IsNullOrEmpty(request.Password) || existingUser.PasswordHash == PasswordHelper.HashPassword(request.Password))
            return;

        if (PasswordHelper.CompareWithLastPassword(existingUser.PasswordHash, request.Password))
            throw new InvalidFieldException("A nova senha não pode ser igual a antiga.");

        existingUser.PasswordHash = PasswordHelper.HashPassword(request.Password);
    }

    private static async Task ValidateEmail(UpdateUserCommand request, GetUserByIdInternalResponse existingUser, IUnitOfWork unitOfWork)
    {
        if (existingUser.Email == request.Email)
            return;

        var existingUserByEmail = await unitOfWork.Users
            .FindAsync(u => u.Email == request.Email && u.UserId != existingUser.UserId);

        if (existingUserByEmail.Any())
            throw new InvalidFieldException("Já existe um usuário com este e-mail.");
    }
    private static async Task ValidateUsername(UpdateUserCommand request, GetUserByIdInternalResponse existingUser, IUnitOfWork unitOfWork)
    {
        if (existingUser.Username == request.Username)
            return;

        var existingUsersByUsername = await unitOfWork.Users.FindAsync(u => u.Username == request.Username && u.UserId != existingUser.UserId);
        if (existingUsersByUsername.Any())
        {
            throw new InvalidFieldException("Já existe um usuário com este nome de usuário.");
        }
        
    }
}