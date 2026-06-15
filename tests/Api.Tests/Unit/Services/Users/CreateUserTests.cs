using Api.Interfaces.Repositories;
using Api.Interfaces.Validators;
using Api.Middlewares;
using Api.Models;
using Api.Security.Passwords;
using Api.Services.Users;
using Api.Tests.TestData.Dtos;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Services.Users;

public class CreateUserTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IUserValidator> _validatorMock;

    private readonly CreateUser _service;

    public CreateUserTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IUserValidator>();

        _service = new CreateUser(
            _repositoryMock.Object,
            _validatorMock.Object
        );
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_ShouldCreateUser_WhenDtoIsValid()
    {
        // Arrange
        var dto = UserCreateDtoFactory.Create();

        User? capturedUser = null;

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .ReturnsAsync((User user) => user);

        // Act
        var result = await _service.ExecuteAsync(dto);

        // Assert

        _validatorMock.Verify(
            v => v.ValidateCreateAsync(dto),
            Times.Once);

        _repositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<User>()),
            Times.Once);

        capturedUser.Should().NotBeNull();

        capturedUser!.Username.Should().Be(dto.Username);
        capturedUser.Email.Should().Be(dto.Email);
        capturedUser.FullName.Should().Be(dto.FullName);

        capturedUser.Active.Should().BeTrue();

        capturedUser.CreatedAt.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(5));

        capturedUser.UpdatedAt.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(5));

        result.Should().BeSameAs(capturedUser);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHashPassword_WhenCreatingUser()
    {
        // Arrange
        var dto = UserCreateDtoFactory.Create();

        User? capturedUser = null;

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .ReturnsAsync((User user) => user);

        // Act
        await _service.ExecuteAsync(dto);

        // Assert

        capturedUser.Should().NotBeNull();

        capturedUser!.Password.Should().NotBe(dto.Password);

        PasswordHash.Verify(
            dto.Password,
            capturedUser.Password)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPropagateException_WhenValidatorFails()
    {
        // Arrange
        var dto = UserCreateDtoFactory.Create();

        _validatorMock
            .Setup(v => v.ValidateCreateAsync(dto))
            .ThrowsAsync(new AppException("Erro de validação"));

        // Act
        Func<Task> act = () => _service.ExecuteAsync(dto);

        // Assert

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Erro de validação");

        _repositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<User>()),
            Times.Never);
    }

    #endregion
}