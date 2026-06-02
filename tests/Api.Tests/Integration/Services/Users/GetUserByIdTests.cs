using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Interfaces.Security.Policies;
using Api.Middlewares;
using Api.Models;
using Api.Services.Users;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Services.Users;

public class GetUserByIdTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IUserVisibilityPolicy> _visibilityMock;

    private readonly GetUserById _service;

    public GetUserByIdTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _visibilityMock = new Mock<IUserVisibilityPolicy>();

        _service = new GetUserById(
            _repositoryMock.Object,
            _visibilityMock.Object
        );
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenIdIsZero()
    {
        Func<Task> act = () => _service.ExecuteAsync(0);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Id inválido.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenIdIsNegative()
    {
        Func<Task> act = () => _service.ExecuteAsync(-1);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Id inválido.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _service.ExecuteAsync(1);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Usuário não encontrado.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserIsNotVisible()
    {
        var user = new User
        {
            Id = 1,
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        _visibilityMock
            .Setup(v => v.CanAccess(user))
            .Returns(false);

        Func<Task> act = () => _service.ExecuteAsync(1);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Usuário não encontrado.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnUserReadDto_WhenUserExistsAndIsVisible()
    {
        var user = new User
        {
            Id = 1,
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        _visibilityMock
            .Setup(v => v.CanAccess(user))
            .Returns(true);

        var result = await _service.ExecuteAsync(1);

        result.Should().NotBeNull();

        result.Id.Should().Be(user.Id);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        result.FullName.Should().Be(user.FullName);
    }

    #endregion
}