using Aurora.Mediator;
using AutoFixture;
using AutoFixture.Xunit2;
using component.template.business.Services.User.Handles;
using component.template.domain.Exceptions;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.domain.Models.External.User;
using component.template.domain.Models.External.User.Commands;
using component.template.domain.Models.Repository;
using NSubstitute;
using Xunit;

namespace component.template.business.test.Services.User.Handles;

public class CreateUserCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _fixture = new Fixture();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateUserCommandHandler(_unitOfWork);
    }

    [Theory, AutoData]
    public async Task Handle_ValidCommand_ShouldCreateUserSuccessfully(
        CreateUserCommand command)
    {
        // Arrange
        var existingUsers = new List<UserDto>();
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(existingUsers);
        
        // Mock para simular que o ID é definido após o commit
        _unitOfWork.Users.AddAsync(Arg.Any<UserDto>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => 
            {
                var user = callInfo.Arg<UserDto>();
                user.UserId = 1; // Simular que o banco define o ID
            });
        
        _unitOfWork.CommitAsync().Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Username, result.Username);
        Assert.Equal(command.Email, result.Email);
        Assert.True(result.IsActive);
        Assert.True(result.Id > 0);
        
        await _unitOfWork.Users.Received(1).AddAsync(Arg.Any<UserDto>());
        await _unitOfWork.Received(1).CommitAsync();
    }

    [Theory, AutoData]
    public async Task Handle_ExistingUsername_ShouldThrowBusinessRuleException(
        CreateUserCommand command,
        UserDto existingUser)
    {
        // Arrange
        var existingUsers = new List<UserDto> { existingUser };
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(existingUsers);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        Assert.Equal("Username já está em uso.", exception.Message);
        
        await _unitOfWork.Users.DidNotReceive().AddAsync(Arg.Any<UserDto>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }

    [Theory, AutoData]
    public async Task Handle_ExistingEmail_ShouldThrowBusinessRuleException(
        CreateUserCommand command,
        UserDto existingUser)
    {
        // Arrange
        var emptyList = new List<UserDto>();
        var existingUsers = new List<UserDto> { existingUser };
        
        // Primeira chamada (username) retorna vazio
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(emptyList, existingUsers);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        Assert.Equal("Email já está em uso.", exception.Message);
        
        await _unitOfWork.Users.DidNotReceive().AddAsync(Arg.Any<UserDto>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }

    [Theory, AutoData]
    public async Task Handle_ValidCommand_ShouldHashPassword(
        CreateUserCommand command)
    {
        // Arrange
        var existingUsers = new List<UserDto>();
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(existingUsers);
        _unitOfWork.CommitAsync().Returns(1);

        UserDto? capturedUserDto = null;
        await _unitOfWork.Users.AddAsync(Arg.Do<UserDto>(x => capturedUserDto = x));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedUserDto);
        Assert.NotEqual(command.Password, capturedUserDto!.PasswordHash);
        Assert.NotEmpty(capturedUserDto!.PasswordHash);
    }

    [Theory, AutoData]
    public async Task Handle_ValidCommand_ShouldSetDefaultValues(
        CreateUserCommand command)
    {
        // Arrange
        var existingUsers = new List<UserDto>();
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(existingUsers);
        _unitOfWork.CommitAsync().Returns(1);

        UserDto? capturedUserDto = null;
        await _unitOfWork.Users.AddAsync(Arg.Do<UserDto>(x => capturedUserDto = x));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedUserDto);
        Assert.True(capturedUserDto!.IsActive);
        Assert.True(capturedUserDto!.CreatedAt <= DateTime.UtcNow);
        Assert.True(capturedUserDto!.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
    }
}