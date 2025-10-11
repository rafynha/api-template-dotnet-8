using AutoFixture;
using AutoFixture.Xunit2;
using component.template.api.business.Services.User.Validations;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Models.External.User.Commands;
using Xunit;

namespace component.template.api.test.Services.User.Validations;

public class CreateUserCommandValidatorTests
{
    private readonly IFixture _fixture;
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new CreateUserCommandValidator();
    }

    [Theory, AutoData]
    public async Task ValidateAsync_ValidCommand_ShouldNotThrowException(
        string username,
        string email,
        string password)
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = username[..Math.Min(username.Length, 50)], // Garantir máximo 50 chars
            Email = $"test@{email[..10]}.com", // Email válido
            Password = $"Valid123!{password[..10]}" // Password válido
        };

        // Act & Assert
        var exception = await Record.ExceptionAsync(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_EmptyUsername_ShouldThrowRequiredFieldException()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, string.Empty)
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "ValidPass123!")
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RequiredFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Username é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_WhitespaceUsername_ShouldThrowRequiredFieldException()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "   ") // Apenas espaços em branco
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "ValidPass123!")
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RequiredFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Username é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_UsernameTooLong_ShouldThrowInvalidFieldException()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, new string('a', 51)) // 51 caracteres
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "ValidPass123!")
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Username deve ter no máximo 50 caracteres.", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_EmptyEmail_ShouldThrowRequiredFieldException()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "testuser")
            .With(x => x.Email, string.Empty)
            .With(x => x.Password, "ValidPass123!")
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RequiredFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Email é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_EmailTooLong_ShouldThrowInvalidFieldException()
    {
        // Arrange
        var longEmail = new string('a', 95) + "@email.com"; // Mais de 100 caracteres
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "testuser")
            .With(x => x.Email, longEmail)
            .With(x => x.Password, "ValidPass123!")
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Email deve ter no máximo 100 caracteres.", exception.Message);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test.example.com")]
    public async Task ValidateAsync_InvalidEmailFormat_ShouldThrowInvalidFieldException(string invalidEmail)
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "testuser")
            .With(x => x.Email, invalidEmail)
            .With(x => x.Password, "ValidPass123!")
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Email deve ter um formato válido.", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_EmptyPassword_ShouldThrowRequiredFieldException()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "testuser")
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, string.Empty)
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RequiredFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Password é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("12345")] // 5 caracteres
    [InlineData("abc")] // 3 caracteres
    [InlineData("1")] // 1 caractere
    public async Task ValidateAsync_PasswordTooShort_ShouldThrowInvalidFieldException(string shortPassword)
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "testuser")
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, shortPassword)
            .Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidFieldException>(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Equal("Password deve ter no mínimo 6 caracteres.", exception.Message);
    }

    [Theory]
    [InlineData("ValidPass123!")]
    [InlineData("Password1@")]
    [InlineData("MySecure987#")]
    public async Task ValidateAsync_ValidPasswords_ShouldNotThrowException(string validPassword)
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(x => x.Username, "testuser")
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, validPassword)
            .Create();

        // Act & Assert
        var exception = await Record.ExceptionAsync(
            () => _validator.ValidateAsync(command, CancellationToken.None));
        
        Assert.Null(exception);
    }
}