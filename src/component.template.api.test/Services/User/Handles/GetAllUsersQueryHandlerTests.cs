using AutoFixture.Xunit2;
using component.template.api.business.Services.User.Handles;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.Common;
using component.template.api.domain.Models.External.User.Queries;
using component.template.api.domain.Models.Repository;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace component.template.api.test.Services.User.Handles;

public class GetAllUsersQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new GetAllUsersQueryHandler(_unitOfWork);
    }

    [Theory, AutoData]
    public async Task Handle_WithoutFilters_ReturnsPagedUsers(
        GetAllUsersQuery query,
        List<UserDto> userList)
    {
        // Arrange
        query.Username = null;
        query.Email = null;
        query.IsActive = null;
        query.PageNumber = 1;
        query.PageSize = 10;

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((userList.AsEnumerable(), userList.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Equal(userList.Count, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var query = new GetAllUsersQuery
        {
            PageNumber = 2,
            PageSize = 5
        };

        // Criar 15 usuários para testar paginação
        var fullUserList = Enumerable.Range(1, 15).Select(i => new UserDto
        {
            UserId = i,
            Username = $"User{i}",
            Email = $"user{i}@example.com",
            IsActive = true,
            CreatedAt = DateTime.Now
        }).ToList();

        // Simular paginação: página 2, 5 itens (índices 5-9)
        var pagedUsers = fullUserList.Skip(5).Take(5).ToList();

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((pagedUsers.AsEnumerable(), 15));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count()); // PageSize = 5
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages); // 15 / 5 = 3
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Theory, AutoData]
    public async Task Handle_WithUsernameFilter_ReturnsFilteredUsers(
        UserDto user1,
        UserDto user2)
    {
        // Arrange
        user1.Username = "JohnDoe";
        user2.Username = "JaneDoe";
        
        var query = new GetAllUsersQuery
        {
            Username = "John",
            PageNumber = 1,
            PageSize = 10
        };

        var userList = new List<UserDto> { user1 };

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((userList.AsEnumerable(), userList.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Contains(result.Items, u => u.Username == user1.Username);
    }

    [Theory, AutoData]
    public async Task Handle_WithEmailFilter_ReturnsFilteredUsers(
        UserDto user1,
        UserDto user2)
    {
        // Arrange
        user1.Email = "john@example.com";
        user2.Email = "jane@example.com";
        
        var query = new GetAllUsersQuery
        {
            Email = "john",
            PageNumber = 1,
            PageSize = 10
        };

        var userList = new List<UserDto> { user1 };

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((userList.AsEnumerable(), userList.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Contains(result.Items, u => u.Email == user1.Email);
    }

    [Theory, AutoData]
    public async Task Handle_WithIsActiveFilter_ReturnsFilteredUsers(
        UserDto user1,
        UserDto user2)
    {
        // Arrange
        user1.IsActive = true;
        user2.IsActive = false;
        
        var query = new GetAllUsersQuery
        {
            IsActive = true,
            PageNumber = 1,
            PageSize = 10
        };

        var userList = new List<UserDto> { user1 };

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((userList.AsEnumerable(), userList.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.All(result.Items, u => Assert.True(u.IsActive));
    }

    [Theory, AutoData]
    public async Task Handle_WithMultipleFilters_ReturnsFilteredUsers(UserDto user1)
    {
        // Arrange
        user1.Username = "JohnDoe";
        user1.Email = "john@example.com";
        user1.IsActive = true;
        
        var query = new GetAllUsersQuery
        {
            Username = "John",
            Email = "john",
            IsActive = true,
            PageNumber = 1,
            PageSize = 10
        };

        var userList = new List<UserDto> { user1 };

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((userList.AsEnumerable(), userList.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var resultUser = result.Items.First();
        Assert.Equal(user1.UserId, resultUser.Id);
        Assert.Equal(user1.Username, resultUser.Username);
        Assert.Equal(user1.Email, resultUser.Email);
        Assert.True(resultUser.IsActive);
    }

    [Theory, AutoData]
    public async Task Handle_WithNoMatchingUsers_ReturnsEmptyPagedResult(GetAllUsersQuery query)
    {
        // Arrange
        query.PageNumber = 1;
        query.PageSize = 10;
        var emptyUserList = new List<UserDto>();

        _unitOfWork.Users.FindPagedAsync(
            Arg.Any<Expression<Func<UserDto, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<UserDto, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<Expression<Func<UserDto, object>>[]>())
            .Returns((emptyUserList.AsEnumerable(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }
}