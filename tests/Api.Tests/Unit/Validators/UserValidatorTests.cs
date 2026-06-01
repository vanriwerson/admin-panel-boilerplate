using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Validations;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Validators;

public class UserValidatorTests
{
    [Fact]
    public async Task ValidateCreateAsync_WhenUsernameAlreadyExists_ShouldThrowAppException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();

        repositoryMock
            .Setup(r => r.ExistsByUsernameAsync("admin"))
            .ReturnsAsync(true);

        repositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var validator = new UserValidator(repositoryMock.Object);

        var dto = new UserCreateDto
        {
            Username = "admin",
            Email = "admin@mail.com",
            Password = "123456",
            FullName = "Administrador",
            PermissionIds = [1]
        };

        // Act
        Func<Task> action =
            () => validator.ValidateCreateAsync(dto);

        // Assert

        await action
            .Should()
            .ThrowAsync<AppException>()
            .WithMessage("Username já está em uso.");
    }
}
