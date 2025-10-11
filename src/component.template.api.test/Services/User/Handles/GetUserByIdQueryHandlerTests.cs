using Aurora.Mediator;
using AutoFixture;
using AutoFixture.Xunit2;
using component.template.api.business.Services.User.Handles;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.External.User;
using component.template.api.domain.Models.External.User.Queries;
using component.template.api.domain.Models.Internal.Profile;
using component.template.api.domain.Models.Internal.Profile.Queries;
using component.template.api.domain.Models.Repository;
using NSubstitute;
using Xunit;

namespace component.template.api.test.Services.User.Handles;

public class GetUserByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _fixture = new Fixture();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mediator = Substitute.For<IMediator>();
        _handler = new GetUserByIdQueryHandler(_unitOfWork, _mediator);
    }

    [Theory, AutoData]
    public async Task Handle_ValidUserId_ShouldReturnUserSuccessfully(
        GetUserByIdQuery query,
        UserDto userDto)
    {
        // Arrange
        var userList = new List<UserDto> { userDto };
        userDto.UserId = query.Id; // Garantir que o ID coincida
        
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(userList);
        
        _mediator.Send(Arg.Any<GetProfileByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetProfileByIdResponse { Id = query.Id, Name = "Test Profile" });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.UserId, result.Id);
        Assert.Equal(userDto.Username, result.Name);
        
        await _mediator.Received(1).Send(
            Arg.Is<GetProfileByIdQuery>(x => x.Id == query.Id), 
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoData]
    public async Task Handle_UserNotFound_ShouldThrowDataNotFoundException(
        GetUserByIdQuery query)
    {
        // Arrange
        var emptyUserList = new List<UserDto>();
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(emptyUserList);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DataNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));
        
        Assert.Contains($"O usuário com id {query.Id} não foi encontrado.", exception.Message);
        
        await _mediator.DidNotReceive().Send(Arg.Any<GetProfileByIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Theory, AutoData]
    public async Task Handle_ValidUserId_ShouldCallProfileService(
        GetUserByIdQuery query,
        UserDto userDto)
    {
        // Arrange
        var userList = new List<UserDto> { userDto };
        userDto.UserId = query.Id;
        
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(userList);
        
        _mediator.Send(Arg.Any<GetProfileByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetProfileByIdResponse { Id = query.Id, Name = "Test Profile" });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _mediator.Received(1).Send(
            Arg.Is<GetProfileByIdQuery>(x => x.Id == query.Id), 
            Arg.Any<CancellationToken>());
    }

    [Theory, AutoData]
    public async Task Handle_MultipleUsersFound_ShouldReturnFirstUser(
        GetUserByIdQuery query,
        UserDto firstUser,
        UserDto secondUser)
    {
        // Arrange
        var userList = new List<UserDto> { firstUser, secondUser };
        firstUser.UserId = query.Id;
        secondUser.UserId = query.Id;
        
        _unitOfWork.Users.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<UserDto, bool>>>())
            .Returns(userList);
        
        _mediator.Send(Arg.Any<GetProfileByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetProfileByIdResponse { Id = query.Id, Name = "Test Profile" });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(firstUser.UserId, result.Id);
        Assert.Equal(firstUser.Username, result.Name);
    }
}