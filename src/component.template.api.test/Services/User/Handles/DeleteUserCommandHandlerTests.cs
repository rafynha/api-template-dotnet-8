using AutoFixture.Xunit2;
using component.template.api.business.Services.User.Handles;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.External.User.Commands;
using component.template.api.domain.Models.Internal.User;
using component.template.api.domain.Models.Internal.User.Queries;
using component.template.api.domain.Models.Repository;
using Aurora.Mediator;
using NSubstitute;
using Xunit;
using NSubstitute.ExceptionExtensions;

namespace component.template.api.test.Services.User.Handles;

public class DeleteUserCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteUserCommandHandler(_unitOfWork, _mediator);
    }

    [Theory, AutoData]
    public async Task Handle_WithValidId_DeletesUserSuccessfully(
        DeleteUserCommand command,
        GetUserByIdInternalResponse userInternalResponse)
    {
        // Arrange
        userInternalResponse.UserId = command.Id;

        _mediator.Send(Arg.Any<GetUserByIdInternalQuery>(), Arg.Any<CancellationToken>())
            .Returns(userInternalResponse);

        _unitOfWork.Users.RemoveAsync(Arg.Any<UserDto>())
            .Returns(1); // Simula sucesso na exclusão

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal("Usuário excluído com sucesso.", result.Message);
        Assert.True(result.DeletedAt <= DateTime.UtcNow);

        await _unitOfWork.Received(1).BeginTransactionAsync();
        await _unitOfWork.Received(1).CommitTransactionAsync();
        await _mediator.Received(1).Send(Arg.Any<GetUserByIdInternalQuery>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Users.Received(1).RemoveAsync(Arg.Any<UserDto>());
    }

    [Theory, AutoData]
    public async Task Handle_WithInvalidId_ThrowsDataNotFoundException(DeleteUserCommand command)
    {
        // Arrange
        _mediator.Send(Arg.Any<GetUserByIdInternalQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new DataNotFoundException("Usuário não encontrado."));

        // Act & Assert
        await Assert.ThrowsAsync<DataNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        await _unitOfWork.Received(1).BeginTransactionAsync();
        await _unitOfWork.Received(1).RollbackAsync();
    }

    [Theory, AutoData]
    public async Task Handle_WhenDeleteFails_ThrowsDatabaseErrorException(
        DeleteUserCommand command,
        GetUserByIdInternalResponse userInternalResponse)
    {
        // Arrange
        userInternalResponse.UserId = command.Id;

        _mediator.Send(Arg.Any<GetUserByIdInternalQuery>(), Arg.Any<CancellationToken>())
            .Returns(userInternalResponse);

        _unitOfWork.Users.RemoveAsync(Arg.Any<UserDto>())
            .Returns(0); // Simula falha na exclusão

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseErrorException>(() =>
            _handler.Handle(command, CancellationToken.None));

        await _unitOfWork.Received(1).BeginTransactionAsync();
        await _unitOfWork.Received(1).RollbackAsync();
    }
}