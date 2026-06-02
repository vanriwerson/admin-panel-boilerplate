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

public class UpdateUserTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IUserValidator> _validatorMock;

    private readonly UpdateUser _service;

    public UpdateUserTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IUserValidator>();

        _service = new UpdateUser(
            _repositoryMock.Object,
            _validatorMock.Object
        );
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateUser_WhenDtoIsValid()
    {
        // Arrange

        var dto = UserUpdateDtoFactory.Create();

        var user = new User
        {
            Id = dto.Id,
            Username = "old",
            Email = "old@email.com",
            FullName = "Old Name",
            Password = PasswordHash.Generate("old-password")
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.UpdateAsync(user))
            .ReturnsAsync(user);

        // Act

        var result = await _service.ExecuteAsync(dto);

        // Assert

        _validatorMock.Verify(
            v => v.ValidateUpdateAsync(dto, user),
            Times.Once);

        _repositoryMock.Verify(
            r => r.UpdateAsync(user),
            Times.Once);

        user.Username.Should().Be(dto.Username);
        user.Email.Should().Be(dto.Email);
        user.FullName.Should().Be(dto.FullName);

        result.Should().BeSameAs(user);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHashPassword_WhenPasswordIsProvided()
    {
        // Arrange

        var dto = UserUpdateDtoFactory.Create();

        var user = new User
        {
            Id = dto.Id,
            Username = "old",
            Email = "old@email.com",
            FullName = "Old Name",
            Password = PasswordHash.Generate("old-password")
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.UpdateAsync(user))
            .ReturnsAsync(user);

        // Act

        await _service.ExecuteAsync(dto);

        // Assert

        user.Password.Should().NotBe(dto.Password);

        PasswordHash.Verify(
            dto.Password!,
            user.Password)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldKeepPassword_WhenPasswordIsNull()
    {
        // Arrange

        var dto = UserUpdateDtoFactory.WithoutPassword();

        var originalPassword =
            PasswordHash.Generate("old-password");

        var user = new User
        {
            Id = dto.Id,
            Username = "old",
            Email = "old@email.com",
            FullName = "Old Name",
            Password = originalPassword
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.UpdateAsync(user))
            .ReturnsAsync(user);

        // Act

        await _service.ExecuteAsync(dto);

        // Assert

        user.Password.Should().Be(originalPassword);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateUpdatedAt()
    {
        // Arrange

        var dto = UserUpdateDtoFactory.Create();

        var oldDate = DateTime.UtcNow.AddDays(-10);

        var user = new User
        {
            Id = dto.Id,
            Username = "old",
            Email = "old@email.com",
            FullName = "Old Name",
            Password = PasswordHash.Generate("old-password"),
            UpdatedAt = oldDate
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.UpdateAsync(user))
            .ReturnsAsync(user);

        // Act

        await _service.ExecuteAsync(dto);

        // Assert

        user.UpdatedAt.Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange

        var dto = UserUpdateDtoFactory.Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(dto.Id))
            .ReturnsAsync((User?)null);

        // Act

        Func<Task> act =
            () => _service.ExecuteAsync(dto);

        // Assert

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Usuário não encontrado.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecuteAsync_ShouldThrow_WhenIdIsInvalid(
        int invalidId)
    {
        // Arrange

        var dto = UserUpdateDtoFactory.Create(
            x => x.Id = invalidId);

        // Act

        Func<Task> act =
            () => _service.ExecuteAsync(dto);

        // Assert

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Id inválido.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPropagateValidatorException()
    {
        // Arrange

        var dto = UserUpdateDtoFactory.Create();

        var user = new User
        {
            Id = dto.Id
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _validatorMock
            .Setup(v => v.ValidateUpdateAsync(dto, user))
            .ThrowsAsync(
                new AppException("Erro de validação"));

        // Act

        Func<Task> act =
            () => _service.ExecuteAsync(dto);

        // Assert

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Erro de validação");

        _repositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    #endregion
}