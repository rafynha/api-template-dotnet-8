using Aurora.Mediator;
using AutoMapper;
using component.template.domain.Common;
using component.template.domain.Exceptions;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.domain.Models.External.User;
using component.template.domain.Models.External.User.Commands;
using component.template.domain.Models.Internal.User;
using component.template.domain.Models.Internal.User.Queries;
using component.template.domain.Models.Repository;

namespace component.template.business.Services.User.Handles;

public class DeleteUserCommandHandler : BaseHandler, IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<GetUserByIdInternalResponse, DeleteUserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Usuário excluído com sucesso."))
            .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        cfg.CreateMap<GetUserByIdInternalResponse, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }

    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Buscar o usuário para verificar se existe usando a query interna
            var existingUser = await _mediator.Send(new GetUserByIdInternalQuery { Id = request.Id }, cancellationToken);

            // Converter para UserDto para operações de repositório
            var userDto = _mapper.Map<UserDto>(existingUser);

            // Excluir o usuário do repositório
            if (await _unitOfWork.Users.RemoveAsync(userDto) <= 0)
                throw new DatabaseErrorException("Erro ao excluir o usuário.");

            await _unitOfWork.CommitTransactionAsync(); // Commit das alterações e encerramento da transação

            // Retornar resposta
            return _mapper.Map<DeleteUserResponse>(existingUser);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}