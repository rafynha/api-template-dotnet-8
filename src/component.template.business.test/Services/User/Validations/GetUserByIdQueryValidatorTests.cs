using AutoFixture;
using AutoFixture.Xunit2;
using component.template.business.Services.User.Validations;
using component.template.domain.Exceptions;
using component.template.domain.Models.External.User.Queries;
using Xunit;

namespace component.template.business.test.Services.User.Validations;

public class GetUserByIdQueryValidatorTests
{
    private readonly IFixture _fixture;
    private readonly GetUserByIdQueryValidator _validator;

    public GetUserByIdQueryValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new GetUserByIdQueryValidator();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(999999)]
    public async Task ValidateAsync_ValidId_ShouldNotThrowException(long validId)
    {
        // Arrange
        var query = _fixture.Build<GetUserByIdQuery>()
            .With(x => x.Id, validId)
            .Create();

        // Act & Assert
        var exception = await Record.ExceptionAsync(
            () => _validator.ValidateAsync(query, CancellationToken.None));
        
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-999)]
    public async Task ValidateAsync_InvalidId_ShouldThrowInvalidFieldException(long invalidId)
    {
        // Arrange
        var query = _fixture.Build<GetUserByIdQuery>()
            .With(x => x.Id, invalidId)
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidFieldException>(
            () => _validator.ValidateAsync(query, CancellationToken.None));
        
        Assert.Equal("Invalid user ID.", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_ZeroId_ShouldThrowInvalidFieldException()
    {
        // Arrange
        var query = _fixture.Build<GetUserByIdQuery>()
            .With(x => x.Id, 0L)
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidFieldException>(
            () => _validator.ValidateAsync(query, CancellationToken.None));
        
        Assert.Equal("Invalid user ID.", exception.Message);
    }

    [Theory, AutoData]
    public async Task ValidateAsync_PositiveId_ShouldCompleteWithoutException(
        GetUserByIdQuery query)
    {
        // Arrange
        query.Id = Math.Abs(query.Id) + 1; // Garantir que seja positivo

        // Act & Assert
        var exception = await Record.ExceptionAsync(
            () => _validator.ValidateAsync(query, CancellationToken.None));
        
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_ShouldCompleteTask()
    {
        // Arrange
        var query = _fixture.Build<GetUserByIdQuery>()
            .With(x => x.Id, 1L)
            .Create();

        // Act
        var task = _validator.ValidateAsync(query, CancellationToken.None);
        
        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        await task; // Garantir que não há exceção
    }
}