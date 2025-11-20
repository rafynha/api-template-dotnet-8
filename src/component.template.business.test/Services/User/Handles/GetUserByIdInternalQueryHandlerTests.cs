using AutoFixture.Xunit2;
using component.template.business.Services.User.Handles;
using component.template.domain.Exceptions;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.domain.Models.Internal.User;
using component.template.domain.Models.Internal.User.Queries;
using component.template.domain.Models.Repository;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace component.template.business.test.Services.User.Handles;

public class GetUserByIdInternalQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetUserByIdInternalQueryHandler _handler;

    public GetUserByIdInternalQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new GetUserByIdInternalQueryHandler(_unitOfWork);
    }

    [Theory, AutoData]
    public async Task Handle_WithValidId_ReturnsGetUserByIdInternalResponse(
        GetUserByIdInternalQuery query,
        UserDto userDto)
    {
        // Arrange
        var userList = new List<UserDto> { userDto };
        userDto.UserId = query.Id; // Garantir que o ID coincida

        _unitOfWork.Users.FindAsync(Arg.Any<Expression<Func<UserDto, bool>>>())
            .Returns(userList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.UserId, result.UserId);
        Assert.Equal(userDto.Username, result.Username);
        Assert.Equal(userDto.Email, result.Email);
        Assert.Equal(userDto.IsActive, result.IsActive);
        Assert.Equal(userDto.CreatedAt, result.CreatedAt);
        Assert.Equal(userDto.UpdatedAt, result.UpdatedAt);
        Assert.Equal(userDto.PasswordHash, result.PasswordHash);
    }

    [Theory, AutoData]
    public async Task Handle_WithInvalidId_ThrowsDataNotFoundException(GetUserByIdInternalQuery query)
    {
        // Arrange
        var emptyUserList = new List<UserDto>();
        _unitOfWork.Users.FindAsync(Arg.Any<Expression<Func<UserDto, bool>>>())
            .Returns(emptyUserList);

        // Act & Assert
        await Assert.ThrowsAsync<DataNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Theory, AutoData]
    public async Task Handle_WithMultipleUsers_ReturnsFirstMatch(
        GetUserByIdInternalQuery query,
        UserDto firstUser,
        UserDto secondUser)
    {
        // Arrange
        var userList = new List<UserDto> { firstUser, secondUser };
        firstUser.UserId = query.Id;
        secondUser.UserId = query.Id;

        _unitOfWork.Users.FindAsync(Arg.Any<Expression<Func<UserDto, bool>>>())
            .Returns(userList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(firstUser.UserId, result.UserId);
        Assert.Equal(firstUser.Username, result.Username);
    }
}