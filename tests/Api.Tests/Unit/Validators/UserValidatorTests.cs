using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Models;
using Api.Validations;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Validators;

public class UserValidatorTests
{
    #region ValidateCreateAsync

    [Fact]
    public async Task ValidateCreateAsync_ShouldPass_WhenDtoIsValid()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        repository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var validator = new UserValidator(repository.Object);

        var dto = new UserCreateDto
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "123456",
            FullName = "Bruno Silva",
            PermissionIds = [1]
        };

        // Act
        var act = async () => await validator.ValidateCreateAsync(dto);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateCreateAsync_ShouldThrow_WhenUsernameAlreadyExists()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var validator = new UserValidator(repository.Object);

        var dto = new UserCreateDto
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "123456",
            FullName = "Bruno Silva",
            PermissionIds = [1]
        };

        // Act
        Func<Task> act = () => validator.ValidateCreateAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Username já está em uso.");
    }

    [Fact]
    public async Task ValidateCreateAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        repository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var validator = new UserValidator(repository.Object);

        var dto = new UserCreateDto
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "123456",
            FullName = "Bruno Silva",
            PermissionIds = [1]
        };

        // Act
        Func<Task> act = () => validator.ValidateCreateAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Email já está em uso.");
    }

    [Fact]
    public async Task ValidateCreateAsync_ShouldThrow_WhenPermissionListIsEmpty()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        repository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var validator = new UserValidator(repository.Object);

        var dto = new UserCreateDto
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "123456",
            FullName = "Bruno Silva",
            PermissionIds = []
        };

        // Act
        Func<Task> act = () => validator.ValidateCreateAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Usuário deve possuir ao menos uma permissão.");
    }

    #endregion

    #region ValidateUpdateAsync

    [Fact]
    public async Task ValidateUpdateAsync_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        repository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var validator = new UserValidator(repository.Object);

        var user = new User
        {
            Username = "old-user",
            Email = "old@email.com"
        };

        var dto = new UserUpdateDto
        {
            Username = "new-user",
            Email = "new@email.com"
        };

        // Act
        var act = async () => await validator.ValidateUpdateAsync(dto, user);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateUpdateAsync_ShouldThrow_WhenUsernameAlreadyExists()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync("new-user"))
            .ReturnsAsync(true);

        var validator = new UserValidator(repository.Object);

        var user = new User
        {
            Username = "old-user",
            Email = "email@email.com"
        };

        var dto = new UserUpdateDto
        {
            Username = "new-user",
            Email = user.Email
        };

        // Act
        Func<Task> act = () => validator.ValidateUpdateAsync(dto, user);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Username já está em uso.");
    }

    [Fact]
    public async Task ValidateUpdateAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(r => r.ExistsByEmailAsync("new@email.com"))
            .ReturnsAsync(true);

        var validator = new UserValidator(repository.Object);

        var user = new User
        {
            Username = "user",
            Email = "old@email.com"
        };

        var dto = new UserUpdateDto
        {
            Username = user.Username,
            Email = "new@email.com"
        };

        // Act
        Func<Task> act = () => validator.ValidateUpdateAsync(dto, user);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Email já está em uso.");
    }

    [Fact]
    public async Task ValidateUpdateAsync_ShouldNotCheckUsername_WhenUsernameDidNotChange()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        var validator = new UserValidator(repository.Object);

        var user = new User
        {
            Username = "bruno",
            Email = "old@email.com"
        };

        var dto = new UserUpdateDto
        {
            Username = "bruno",
            Email = "new@email.com"
        };

        // Act
        await validator.ValidateUpdateAsync(dto, user);

        // Assert
        repository.Verify(
            r => r.ExistsByUsernameAsync(It.IsAny<string>()),
            Times.Never()
        );
    }

    [Fact]
    public async Task ValidateUpdateAsync_ShouldNotCheckEmail_WhenEmailDidNotChange()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();

        var validator = new UserValidator(repository.Object);

        var user = new User
        {
            Username = "old-user",
            Email = "same@email.com"
        };

        var dto = new UserUpdateDto
        {
            Username = "new-user",
            Email = "same@email.com"
        };

        repository
            .Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        await validator.ValidateUpdateAsync(dto, user);

        // Assert
        repository.Verify(
            r => r.ExistsByEmailAsync(It.IsAny<string>()),
            Times.Never()
        );
    }

    #endregion
}